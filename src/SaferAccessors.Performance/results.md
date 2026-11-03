## FieldUsage

| Method                             | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|----------:|------------:|
| GetAndSetFieldsViaReflection       | 31.3900 ns | 0.4724 ns | 0.4419 ns | 31.2206 ns | 1.000 |    0.02 | 0.0069 |     120 B |        1.00 |
| GetAndSetFieldsViaCachedReflection | 20.3221 ns | 0.1180 ns | 0.1104 ns | 20.3393 ns | 0.648 |    0.01 | 0.0069 |     120 B |        1.00 |
| GetAndSetFieldsViaUnsafeAccessors  |  0.0002 ns | 0.0005 ns | 0.0005 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |        0.00 |

## MethodUsage

| Method                     | Mean       | Error     | StdDev    | Median     | Ratio | Gen0   | Allocated | Alloc Ratio |
|--------------------------- |-----------:|----------:|----------:|-----------:|------:|-------:|----------:|------------:|
| ComputeViaReflection       | 21.0773 ns | 0.1664 ns | 0.1557 ns | 21.0672 ns | 1.000 | 0.0046 |      80 B |        1.00 |
| ComputeViaCachedReflection | 10.8356 ns | 0.1550 ns | 0.1450 ns | 10.8313 ns | 0.514 | 0.0046 |      80 B |        1.00 |
| ComputeViaUnsafeAccessors  |  0.0010 ns | 0.0018 ns | 0.0017 ns |  0.0000 ns | 0.000 |      - |         - |        0.00 |

## PropertyUsage

| Method                                 | Mean       | Error     | StdDev    | Median     | Ratio | Gen0   | Allocated | Alloc Ratio |
|--------------------------------------- |-----------:|----------:|----------:|-----------:|------:|-------:|----------:|------------:|
| GetAndSetPropertiesViaReflection       | 36.8745 ns | 0.3305 ns | 0.2580 ns | 36.8924 ns | 1.000 | 0.0069 |     120 B |        1.00 |
| GetAndSetPropertiesViaCachedReflection | 26.0244 ns | 0.1143 ns | 0.1069 ns | 26.0107 ns | 0.706 | 0.0069 |     120 B |        1.00 |
| GetAndSetPropertiesViaUnsafeAccessors  |  0.0004 ns | 0.0011 ns | 0.0010 ns |  0.0000 ns | 0.000 |      - |         - |        0.00 |

## ConstructorUsage

| Method                    | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------------- |-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| CreateViaReflection       | 49.4741 ns | 0.8820 ns | 0.8250 ns | 1.000 |    0.02 | 0.0106 |     184 B |        1.00 |
| CreateViaCachedReflection |  8.8730 ns | 0.0615 ns | 0.0513 ns | 0.179 |    0.00 | 0.0032 |      56 B |        0.30 |
| CreateViaUnsafeAccessors  |  0.0213 ns | 0.0096 ns | 0.0089 ns | 0.000 |    0.00 |      - |         - |        0.00 |