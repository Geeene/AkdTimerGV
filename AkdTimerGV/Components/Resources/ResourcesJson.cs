namespace AkdTimerGV.Components.Resources {
    public class ResourcesJson {
        public List<Category> categories { get; set; }

        public Dictionary<String, int> getAllTags() { 
            List<String> tags = new List<String>();

            foreach (Category category in categories) {
                tags.AddRange(category.recursivelyGetTags());
            }

            return tags.GroupBy(tag => tag).ToDictionary(group => group.Key, group => group.Count());
        }
    }

    public class Category {
        public string iconPath { get; set; }
        public List<Category> subCategories { get; set; } = [];
        public String categoryName { get; set; }
        public List<CategoryEntry> entries { get; set; } = [];

        public List<String> recursivelyGetTags() {
            List<String> tags = new List<String>();

            foreach (Category category in subCategories) {
                tags.AddRange(category.recursivelyGetTags());
            }

            foreach (CategoryEntry entry in entries) {
                if (entry.tags.Count > 0) { 
                    tags.AddRange(entry.tags);
                }
            }

            return tags;
        }
    }

    public class CategoryEntry {
        /// <summary>
        /// Link to a referenced resource
        /// </summary>
        public string name { get; set; }
        public string reference { get; set; }
        public string credit { get; set; }
        public string description { get; set; }
        public List<String> tags { get; set; }
    }
}
