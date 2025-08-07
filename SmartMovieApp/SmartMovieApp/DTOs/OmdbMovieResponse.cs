namespace SmartMovieApp.DTOs
{
    public class OmdbMovieResponse
    {
        public string Title { get; set; }
        public string Plot { get; set; }
        public string Poster { get; set; }
        public string Released { get; set; } // "24 May 2019"
        public string imdbRating { get; set; }
        public string Response { get; set; }
    }

}
