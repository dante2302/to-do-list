using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ToDoDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<ToDoTask> Tasks { get; set; }
}