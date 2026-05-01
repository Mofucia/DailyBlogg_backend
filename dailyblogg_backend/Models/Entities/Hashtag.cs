using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace dailyblogg_backend.Models.Entities
{
    public class Hashtag
    {
        public int HashtagId { get; set; }
        public string HashtagName { get; set; } = string.Empty;
        public virtual ICollection<Post> PostHashtags { get; set; } = new List<Post>();
    }
}
