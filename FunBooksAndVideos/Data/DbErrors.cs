namespace FunBooksAndVideos.Data
{
    public enum DbErrors : byte
    {
        None = 0,

        General = 1,

        BadConnectionString = 2,

        DatabaseDoesNotExist = 3,

        DifferencesInMigration = 4
    }
}