using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.RegularExpressions;

namespace ApprovalManagement.Models
{
    public enum ProjectStatusValue
    {
        Preparation,
        ForBudgetApproval,
        Ongoing,
        Done
    }
    //Pending, remove Waiting
    public enum ApprovalStatusValue
    {
        None = 0,
        Waiting,
        Pending,
        Approved,
        Declined
    }


    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        [DisplayName("Project ID")]
        public int ProjectId { get; set; }


        [Column(TypeName = "nvarchar(30)")]
        [DisplayName("Project Name")]
        [Required(ErrorMessage = "This field is required.")]
        public string ProjectName { get; set; }


        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "This field is required.")]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DisplayName("Start Date")]
        [Required(ErrorMessage = "This field is required.")]
        public DateTime StartDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DisplayName("End Date")]
        [Required(ErrorMessage = "This field is required.")]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "int")]
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Planned Budget")]
        public int PlannedBudget { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "This field is required.")]
        public string Manager { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [DisplayName("Project Status")]
        [ValidateNever]
        [BindNever]
		public ProjectStatusValue ProjectStatus { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [DisplayName("Approval Status")]
		[ValidateNever]
        [BindNever]
        public ApprovalStatusValue ApprovalStatus { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Approval Remarks")]
      
        public string? ApprovalRemarks { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DisplayName("Approval Date")]
        public DateTime DateOfApproval { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        [DisplayName("Approval Report Path")]
        public string? ApprovalReportPath { get; set; }


        public Project()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
        }

    }
}
