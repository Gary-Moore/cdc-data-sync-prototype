using System.ComponentModel.DataAnnotations;

namespace CdcDataSyncPrototype.BusinessApp.ViewModels;

public class PublicationViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = default!;

    [Required]
    [StringLength(100)]
    public string Type { get; set; } = default!;

    [Display(Name = "Internal Only")]
    public bool InternalOnly { get; set; }

    [Display(Name = "Publish Start Date")]
    [DataType(DataType.Date)]
    public DateTime? PublishStartDate { get; set; }

    [Display(Name = "Publish End Date")]
    [DataType(DataType.Date)]
    public DateTime? PublishEndDate { get; set; }

    [Display(Name = "Last Modified")]
    [DataType(DataType.DateTime)]
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    public string SyncStatus
    {
        get
        {
            if (InternalOnly)
                return "Suppressed due to InternalOnly";

            if (PublishStartDate > DateTime.UtcNow)
                return "Not yet due (PublishStartDate)";

            if (PublishEndDate.HasValue && PublishEndDate < DateTime.UtcNow)
                return "Expired (PublishEndDate)";

            return "Will sync";
        }
    }

}
