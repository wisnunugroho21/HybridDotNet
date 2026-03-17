using General.Common.Services;
using General.Data;
using General.Data.Types;
using Microsoft.Extensions.Logging;

namespace General.Features;

public class TodoService(AppDbContext context, ILogger<TodoService> logger) : CrudService<Todo>(context, logger)
{
    
}