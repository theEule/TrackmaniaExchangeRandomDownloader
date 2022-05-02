namespace HelloWorld
{
    public class Result
    {
        public int TrackID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public string GbxMapName { get; set; }
        public string AuthorLogin { get; set; }
        public string MapType { get; set; }
        public string TitlePack { get; set; }
        public string TrackUID { get; set; }
        public string Mood { get; set; }
        public int DisplayCost { get; set; }
        public string ModName { get; set; }
        public int Lightmap { get; set; }
        public string ExeVersion { get; set; }
        public string ExeBuild { get; set; }
        public int AuthorTime { get; set; }
        public int ParserVersion { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public string TypeName { get; set; }
        public string StyleName { get; set; }
        public string EnvironmentName { get; set; }
        public string VehicleName { get; set; }
        public bool UnlimiterRequired { get; set; }
        public string RouteName { get; set; }
        public string LengthName { get; set; }
        public string DifficultyName { get; set; }
        public int Laps { get; set; }
        public int? ReplayWRID { get; set; }
        public int? ReplayWRTime { get; set; }
        public int? ReplayWRUserID { get; set; }
        public string ReplayWRUsername { get; set; }
        public int TrackValue { get; set; }
        public string Comments { get; set; }
        public int MappackID { get; set; }
        public bool Unlisted { get; set; }
        public bool Unreleased { get; set; }
        public bool Downloadable { get; set; }
        public int RatingVoteCount { get; set; }
        public double RatingVoteAverage { get; set; }
        public bool HasScreenshot { get; set; }
        public bool HasThumbnail { get; set; }
        public bool HasGhostBlocks { get; set; }
        public int EmbeddedObjectsCount { get; set; }
        public int EmbeddedItemsSize { get; set; }
        public int AuthorCount { get; set; }
        public bool IsMP4 { get; set; }
        public bool SizeWarning { get; set; }
        public int AwardCount { get; set; }
        public int CommentCount { get; set; }
        public int ReplayCount { get; set; }
        public int ImageCount { get; set; }
        public int VideoCount { get; set; }
    }

    public class Root
    {
        public List<Result> results { get; set; }
        public int totalItemCount { get; set; }
    }
}