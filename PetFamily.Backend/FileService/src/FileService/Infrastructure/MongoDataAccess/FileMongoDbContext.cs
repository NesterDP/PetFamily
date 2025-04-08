using FileService.Core.Models;
using MongoDB.Driver;

namespace FileService.Infrastructure.MongoDataAccess;

public class FileMongoDbContext
{
    private readonly IMongoDatabase _database;
    private const string FILES_COLLECTION = "files";
    private const string DATABASE_NAME = "files_db";
    
    public IMongoCollection<FileData> Files => _database.GetCollection<FileData>(FILES_COLLECTION);
    
    public FileMongoDbContext(IMongoClient mongoClient)
    {
        _database = mongoClient.GetDatabase(DATABASE_NAME);
    }
}