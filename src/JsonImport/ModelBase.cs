using System.ComponentModel.DataAnnotations;

namespace DynamicModel {
    public class ModelBase {
        [Key]
        public int Auto { set; get; } = 0;
    }
}
