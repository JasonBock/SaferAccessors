## FieldUsage

| Method                             | Mean       | Error     | StdDev    | Median     | Ratio | Gen0   | Allocated | Alloc Ratio |
|----------------------------------- |-----------:|----------:|----------:|-----------:|------:|-------:|----------:|------------:|
| GetAndSetFieldsViaReflection       | 30.4837 ns | 0.1732 ns | 0.1536 ns | 30.4708 ns | 1.000 | 0.0069 |     120 B |        1.00 |
| GetAndSetFieldsViaCachedReflection | 20.8945 ns | 0.4318 ns | 0.3828 ns | 21.0489 ns | 0.685 | 0.0069 |     120 B |        1.00 |
| GetAndSetFieldsViaUnsafeAccessors  |  0.0000 ns | 0.0001 ns | 0.0001 ns |  0.0000 ns | 0.000 |      - |         - |        0.00 |

## MethodUsage

| Method                     | Mean       | Error     | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
|--------------------------- |-----------:|----------:|----------:|------:|-------:|----------:|------------:|
| ComputeViaReflection       | 21.0474 ns | 0.1311 ns | 0.1162 ns | 1.000 | 0.0046 |      80 B |        1.00 |
| ComputeViaCachedReflection | 11.2762 ns | 0.0869 ns | 0.0725 ns | 0.536 | 0.0046 |      80 B |        1.00 |
| ComputeViaUnsafeAccessors  |  0.0081 ns | 0.0094 ns | 0.0088 ns | 0.000 |      - |         - |        0.00 |

## PropertyUsage

| Method                                 | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|----------:|------------:|
| GetAndSetPropertiesViaReflection       | 38.1893 ns | 0.7687 ns | 0.8544 ns | 38.3957 ns | 1.000 |    0.03 | 0.0069 |     120 B |        1.00 |
| GetAndSetPropertiesViaCachedReflection | 27.2610 ns | 0.1754 ns | 0.1641 ns | 27.2966 ns | 0.714 |    0.02 | 0.0069 |     120 B |        1.00 |
| GetAndSetPropertiesViaUnsafeAccessors  |  0.0090 ns | 0.0078 ns | 0.0073 ns |  0.0134 ns | 0.000 |    0.00 |      - |         - |        0.00 |

## ConstructorUsage

| Method                    | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|----------:|------------:|
| CreateViaReflection       | 49.2450 ns | 0.6817 ns | 0.6376 ns | 49.0103 ns | 1.000 |    0.02 | 0.0106 |     184 B |        1.00 |
| CreateViaCachedReflection | 10.5868 ns | 0.0473 ns | 0.0419 ns | 10.6044 ns | 0.215 |    0.00 | 0.0032 |      56 B |        0.30 |
| CreateViaUnsafeAccessors  |  0.0349 ns | 0.0216 ns | 0.0202 ns |  0.0270 ns | 0.001 |    0.00 |      - |         - |        0.00 |