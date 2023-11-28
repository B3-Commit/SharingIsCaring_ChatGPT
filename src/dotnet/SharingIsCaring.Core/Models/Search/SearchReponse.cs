namespace SharingIsCaring.Core.Models.Search;

public class SearchResponse
{
    public bool SCopeRemoved { get; set; } = false;
    public int Count { get; set; }
    public string NextLink { get; set; } = null!;
    public string SrchEng { get; set; } = null!;
    public bool TermHasSynonyms { get; set; }

    public IEnumerable<Facet> Facets { get; set; }
        = Enumerable.Empty<Facet>();    

    public IEnumerable<Result> Results { get; set; }
        = Enumerable.Empty<Result>();

    public class Facet
    {
        public IEnumerable<Product> Products { get; set; }
            = Enumerable.Empty<Product>();

        // TODO: Implement tags

        public IEnumerable<CategoryValue> Category { get; set; }
            = Enumerable.Empty<CategoryValue>();

        public class CategoryValue
        {
            public int Count { get; set; }
            public string Value { get; set; } = null!;
        }

        public class Product 
        {
            public string DisplayName { get; set; } = null!;
            public int Count { get; set; }
            public string Value { get; set; } = null!;
            public string Type { get; set; } = null!;

            public IEnumerable<Product> Children { get; set; }
                = Enumerable.Empty<Product>();
        }
    }

    public class Result
    {
        public string Title { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime LastUpdatedDate { get; set; }
        public string Category { get; set; } = null!;

        // TODO: Implement breadcrumbs
        
        public DisplayUrlValue DisplayUrl { get; set; } = null!;
        public IEnumerable<DescriptionValue> Descriptions { get; set; }
            = Enumerable.Empty<DescriptionValue>();

        public class DisplayUrlValue
        {
            public string Content { get; set; } = null!;
            public IEnumerable<HighLight> HitHighLights { get; set; }
                = Enumerable.Empty<HighLight>();
        }

        public class DescriptionValue
        {
            public string Content { get; set; } = null!;
            public IEnumerable<HighLight> HitHighLights { get; set; }
                = Enumerable.Empty<HighLight>();
        }

        public class HighLight
        {
            public int Start { get; set; }
            public int Lenght { get; set; }
        }
    }
}