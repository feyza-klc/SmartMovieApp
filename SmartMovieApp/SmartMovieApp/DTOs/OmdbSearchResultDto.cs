namespace SmartMovieApp.DTOs
{
    public class OmdbSearchResultDto
    {
        public List<MovieDto> Search {  get; set; }
        public string TotalResults {  get; set; }
        public string Response {  get; set; }
    }
}
