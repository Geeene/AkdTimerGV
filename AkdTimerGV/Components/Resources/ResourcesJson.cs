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

        public Dictionary<String, int> getAllGameFilters() { 
            List<String> filters = new List<String>();

            foreach (Category category in categories) {
                filters.AddRange(category.recursivelyGetGameFilter());
            }

            return filters.GroupBy(tag => tag).ToDictionary(group => group.Key, group => group.Count());
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

        public List<String> recursivelyGetGameFilter() {
            List<String> tags = new List<String>();

            foreach (Category category in subCategories) {
                tags.AddRange(category.recursivelyGetGameFilter());
            }

            foreach (CategoryEntry entry in entries) {
                if (entry.games.Count > 0) { 
                    tags.AddRange(entry.games);
                }
            }

            return tags;
        }

        public List<CategoryEntry> getAllEntries() {
            List<CategoryEntry> AllEntries = new List<CategoryEntry>();
            foreach (Category category in subCategories) {
                AllEntries.AddRange(category.getAllEntries());
            }
            AllEntries.AddRange(this.entries);
            return AllEntries;
        }

        public bool hasAnyEntryToShow(Dictionary<String, bool> TagDisplayDict, Dictionary<String, bool> GameFilterDict) {
            return getAllEntries().Find(ce => ce.shouldBeShown(TagDisplayDict, GameFilterDict)) != null;
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
        public List<String> games { get; set; } = [];
        public List<String> tags { get; set; } = [];

        public bool shouldBeShown(Dictionary<String, bool> TagDisplayDict, Dictionary<String, bool> GameFilterDict) {
            // By default all tags are unchecked, if so, we still want to show everything
            bool noTagSelected = TagDisplayDict.Values.Where(v => v == true).Any() == false;
            bool noGameSelected = GameFilterDict.Values.Where(v => v == true).Any() == false;
            if (noTagSelected && noGameSelected) {
                return true;
            }

            bool showByTags = noTagSelected ||
                // If the entry has no tags, just show it
                this.tags.Count == 0
                // Otherwise, only show the entry if atleast one of it's tags should be displayed
                || this.tags.Where(tag => TagDisplayDict.GetValueOrDefault(tag, true) == true).Any();

            bool showByGame = noGameSelected 
                || this.games.Count == 0 
                || this.games.Where(game => GameFilterDict.GetValueOrDefault(game, true) == true).Any();

            return showByGame && showByTags;
        }
    }
}
