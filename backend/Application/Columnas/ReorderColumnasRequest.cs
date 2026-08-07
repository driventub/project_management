namespace ProjectManagement.Application.Columnas;

public record ReorderColumnaItem(Guid Id, int Orden);

public record ReorderColumnasRequest(IReadOnlyList<ReorderColumnaItem> Items);
