# Análisis de Accidentes de Tráfico de Madrid (LINQ vs DataFrame)

Aplicación de consola en C# / .NET 10 que compara dos formas de analizar datos tabulares grandes: colecciones en memoria con **LINQ** y estructuras columnares con **DataFrame** (`Microsoft.Data.Analysis`). Carga los partes de accidentes de tráfico de Madrid de 2024 a 2026, ejecuta el mismo conjunto de 30 consultas con ambas técnicas y mide cuánto tarda cada una.

Origen de los datos: [Portal de datos abiertos del Ayuntamiento de Madrid](https://datos.madrid.es/dataset/300228-0-accidentes-trafico-detalle/information)

## Cómo ejecutarlo

**Con .NET instalado:**
```bash
dotnet run --project AccidentesMadrid
```

**Con Docker:**
```bash
docker compose up --build
```

Los tres CSV tienen que estar en `data/`, al mismo nivel que el `.csproj`. El proyecto los copia solo a la carpeta de compilación (y a la imagen de Docker), no hace falta moverlos a mano.

## Organización del código

```
AccidentesMadrid/
├── Program.cs
├── appsettings.json
├── Dockerfile
├── docker-compose.yml
├── data/                    → Accidentalidad-2024.csv, 2025.csv, 2026.csv
├── Mappers/                 → AccidenteMapper.cs
├── Models/                  → Accidente.cs, Sexo.cs, TipoPersona.cs
├── Repositories/            → AccidentesRepository.cs, IAccidentesRepository.cs
└── Services/                → AccidentesLinqAnalyzer.cs, AccidentesDataFrameAnalyzer.cs, IAccidentesAnalyzer.cs
```

## Decisiones tomadas y por qué

**Parseo de los CSV con CsvHelper en vez de `Split(';')` manual**
Al principio leía el CSV separando cada línea por `;` a mano. Se rompía en algunas filas porque el fichero de 2026 trae las cabeceras con espacios de relleno (`" fecha     "` en vez de `"fecha"`), y además el campo `localizacion` puede desalinear columnas si no se parsea bien. CsvHelper resuelve esto sin tener que ir parcheando casos sueltos.

**Carga de los 3 ficheros en paralelo**
`CargarTodosParaleloAsync` lanza un `Task.Run` por cada CSV y espera con `Task.WhenAll`. Como los tres ficheros no dependen entre sí (30.000-50.000 filas cada uno), tiene sentido leerlos a la vez en vez de uno detrás de otro.

**Cada fila es una persona, no un accidente**
El total cargado es 130.864 filas, pero eso son personas implicadas (conductores, peatones, pasajeros), no accidentes únicos — un mismo accidente puede generar varias filas si hay varias personas involucradas. Esto es relevante para interpretar bien los conteos.

**`Accidente` como `record` con propiedades `init` y campos calculados**
Los datos no cambian una vez cargados, así que `init` en vez de `set` tiene sentido. Año, mes, día de la semana y "es fin de semana" se calculan a partir de `Fecha` como propiedades de solo lectura, para no repetir esa lógica en cada una de las 30 consultas.

**Mismo contrato para las dos técnicas**
`AccidentesLinqAnalyzer` y `AccidentesDataFrameAnalyzer` tienen los mismos 30 métodos con la misma firma (uno recibe `IEnumerable<Accidente>`, el otro `DataFrame`), y cada uno tiene un `ImprimirResultados()` que saca todo por consola. Así `Program.cs` se queda en cargar datos, medir tiempos y llamar a los dos, sin mezclar cálculo con impresión.

**Consultas sobre el DataFrame con bucles simples**
Para contar valores por columna (`ContarPorColumna`) y para filtrar (`FiltrarPorValor`) recorro las filas con un `for` normal y acumulo en un `Dictionary` o una lista de índices, en vez de usar la API más avanzada de agregación del propio DataFrame. Es más código, pero es el mismo tipo de lógica que ya se usa en LINQ, y con eso me aseguro de entender exactamente qué hace cada consulta.

## Tiempos medidos

| Fase | Tiempo |
|---|---:|
| Carga de los 3 CSV (en paralelo) | 1.148 ms |
| Construir el DataFrame | 403 ms |
| 30 consultas LINQ | 751 ms |
| 30 consultas DataFrame | 2.254 ms |
| DataFrame total (construir + consultar) | 2.657 ms |

(Sobre 130.864 registros. Los tiempos cambian un poco entre ejecuciones por el JIT y el estado del sistema, pero el orden de magnitud se mantiene.)

## Por qué salen estos tiempos

LINQ trabaja directo sobre la lista de objetos que ya está en memoria: cada consulta es un recorrido con `GroupBy`/`Where`/`Count`, sin nada previo que preparar.

El DataFrame necesita un paso extra antes de poder consultar nada: convertir las 130.864 filas de objetos a columnas tipadas, lo que cuesta 403 ms él solo. Y luego, cada vez que filtro (`FiltrarPorValor`), no solo cuento filas — construyo un DataFrame nuevo con las filas que cumplen la condición, lo cual es más trabajo que un `Where` de LINQ que simplemente recorre y descarta. Esto pesa especialmente en las consultas que van año por año, porque ahí filtro el DataFrame completo una vez por cada uno de los 3 años.

**Los resultados de las 30 consultas coinciden entre LINQ y DataFrame**, lo cual no era obvio de antemano porque son dos implementaciones completamente separadas — que lleguen al mismo número en todo es una buena señal de que tanto el parseo como la lógica están bien.

**Un resultado que llamó la atención:** positivos en droga sale 0 en las 30 consultas, en los tres años. Lo comprobé directamente sobre los CSV con PowerShell (`Select-String -Pattern ";S$"`) antes de asumir que era un bug, y en efecto no hay ni un solo "S" en esa columna en ninguno de los tres ficheros. No es un fallo de parseo — el dataset de Madrid simplemente no tiene ese dato relleno en este periodo, a diferencia de `positiva_alcohol`, que sí registra 3.572 casos.

## Qué me llevo de esto

Con este volumen de datos (130.000 filas) y viniendo de objetos ya cargados en memoria, LINQ gana en tiempo total. El DataFrame tendría más sentido si los datos ya vinieran en formato tabular desde el origen (sin pasar por una lista de objetos primero) o si el volumen fuera mucho mayor y las operaciones fueran principalmente agregaciones sobre columnas completas, no filtrados repetidos. Para la próxima vez, mediría también el tiempo de cada consulta por separado (no solo el total por fase), porque ahora mismo sé que el filtrado es el cuello de botella del DataFrame, pero no exactamente cuánto pesa cada consulta individual.