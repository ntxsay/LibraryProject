Scaffold-DbContext "Data Source=Sql/LibraryDB.db" Microsoft.EntityFrameworkCore.Sqlite  -Context LibraryDbContext -OutputDir Models/Local -Force
dotnet ef dbcontext scaffold "Data Source=Scaffold-DbContext "Data Source=/Code/Sql/RostalDB.db" Microsoft.EntityFrameworkCore.Sqlite -o Models/Local -f -c RostalDbContext
Scaffold-DbContext "Data Source=LNA.db" Microsoft.EntityFrameworkCore.Sqlite  -Context DMADbContext -OutputDir Models/Local -Force
Scaffold-DbContext "Data Source=LNA.db" Microsoft.EntityFrameworkCore.Sqlite  -Context DMADbContext -OutputDir Models/Local
dotnet ef dbcontext scaffold "Data Source=LNA.db" Microsoft.EntityFrameworkCore.Sqlite -o Models/Local -f -c DMADbContext

<# StorageFolder localFolder = ApplicationData.Current.LocalFolder;
var file = localFolder.GetFileAsync("DMA.db").AsTask().GetAwaiter().GetResult();
if (file == null) return;
optionsBuilder.UseSqlite($"Data Source={file.Path}"); 
.ValueGeneratedOnAdd();
#>