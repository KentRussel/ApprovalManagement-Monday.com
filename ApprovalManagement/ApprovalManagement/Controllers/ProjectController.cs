using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using ApprovalManagement.Models;
using System.Net.Mail;
using System.Net;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office.Word;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using Document = Microsoft.Office.Interop.Word.Document;
using System.IO;
using PdfSharp.Pdf;
using System.IO;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace ApprovalManagement.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ProjectDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProjectController(ProjectDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
              return View(await _context.Projects.ToListAsync());
        }

        // GET: Create/Create
        public IActionResult Create()
        {
            return View(new Project());
        }

        // POST: Create/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(
            "ProjectId,ProjectName,Description,StartDate,EndDate,PlannedBudget,Manager,ProjectStatus")] Project project)
        {
			if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Project/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Project/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,ProjectName,Description,StartDate,EndDate,PlannedBudget,Manager,ProjectStatus,ApprovalStatus,ApprovalRemarks")] Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (project.ApprovalRemarks == null)
                    {
                        project.ApprovalRemarks = "";
                    }
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'ProjectDbContext.Projects'  is null.");
            }
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
          return _context.Projects.Any(e => e.ProjectId == id);
        }

        private void SendEmail(string recipient, string subject, string body, bool isBodyHtml = true, List<string> attachmentFilePaths = null)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("dbtird.intern1@gmail.com", "ijjshosbsiupxxaf");
            smtpClient.EnableSsl = true;

            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = new MailAddress("dbtird.intern1@gmail.com");
            mailMessage.To.Add(recipient);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = isBodyHtml;

            if (attachmentFilePaths != null && attachmentFilePaths.Any())
            {
                foreach (var attachmentFilePath in attachmentFilePaths)
                {
                    Attachment attachment = new Attachment(attachmentFilePath);
                    mailMessage.Attachments.Add(attachment);
                }
            }

            smtpClient.Send(mailMessage);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateProjectStatus(int projectId, ProjectStatusValue status)
        {
            // update database record with projectId and projectStatus
            var project = await _context.Projects.FindAsync(projectId);

            if (project.ProjectStatus != status)
            {
                project.ProjectStatus = status;
                await _context.SaveChangesAsync();

                if (status == ProjectStatusValue.ForBudgetApproval)
                {
                    // send email to notify stakeholders that the project is ready for budget approval
                    var approvalLink = Url.Action("ApprovalForm", "Project", new { projectId = project.ProjectId }, Request.Scheme);
                    var recipients = "villabroza.hanssell@gmail.com";
                    var subject = project.ProjectName + " - Budget Approval";
                    var body = $@"
                        <p>Hello Direc!</p>
                        <p>We are requesting for your approval on our new project - {project.ProjectName}</p>
                        <p>Please see the details below for your reference.</p><br>
                        <p><b>Project Name:</b> {project.ProjectName}</p>
                        <p><b>Description:</b> {project.Description}</p>
                        <p><b>Timeline:</b> {project.StartDate.ToString("MMM dd, yyyy")} - {project.EndDate.ToString("MMM dd, yyyy")}</p>
                        <p><b>Manager:</b> {project.Manager}</p>
                        <p><b>Budget:</b> {project.PlannedBudget}</p><br>
                        
                        <!-- Button to open the approval form -->
                        <div style=""display: inline-block;"">
                          <form method='post' action='{approvalLink}'>
                            <button type='submit' name='approvalResponse' value='Approve' style='background-color: green; height: 30px; width: 100px; color: white;'>Approve</button>
                          </form>
                        </div>

                        <!-- Button to open the decline form -->
                        <div style=""display: inline-block;"">
                          <form method='post' action='{approvalLink}'>
                            <button type='submit' name='approvalResponse' value='Decline' style='background-color: red; height: 30px; width: 100px; color: white;'>Decline</button>
                          </form>
                        </div>
                    ";
                    SendEmail(recipients, subject, body, true, new List<string>());
                }

                if (status == ProjectStatusValue.ForBudgetApproval)
                {
                    project.ApprovalStatus = ApprovalStatusValue.Pending; // Update approval status to pending
                                                                          // send email and other relevant code...
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // redirect back to the index page
            } 

            return RedirectToAction("Index"); // redirect back to the index page
        }
        
        //GET: Project Details based - projectId reference  
        public async Task<IActionResult> ApprovalForm(int projectId, string approvalResponse, string approvalstatus)
        {
            try
            {
                var project = await _context.Projects.FindAsync(projectId);
                if (project == null)
                {
                    return NotFound();
                }

                approvalstatus = project.ApprovalStatus.ToString();
                ViewData["ApprovalStatus"] = approvalstatus;
                ViewData["ApprovalResponse"] = approvalResponse;
                return View(project);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error view or redirect to an error page
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateApprovalStatus(int projectId, ApprovalStatusValue approvalstatus, string remarks)
        {
            if (ModelState.IsValid)
            {
                var project = await _context.Projects.FindAsync(projectId);

                if (project == null)
                {
                    return NotFound();
                }

                if (approvalstatus == ApprovalStatusValue.Approved)
                {
                    project.ApprovalStatus = ApprovalStatusValue.Approved;
                }

                else if (approvalstatus == ApprovalStatusValue.Declined)
                {
                    project.ApprovalStatus = ApprovalStatusValue.Declined;
                }
                project.ApprovalRemarks = remarks;
                project.DateOfApproval = DateTime.Now;
                project.ProjectStatus = ProjectStatusValue.Ongoing;

                //Generating Report after approval status is processed (Approved/Declined)
                if (project.ApprovalStatus == ApprovalStatusValue.Approved || project.ApprovalStatus == ApprovalStatusValue.Declined)
                {
                    // Get the path to the template and report files
                    string templatePath = "wwwroot/templates/projectApproval.docx";
                    string templateFullPath = Path.Combine(_env.ContentRootPath, templatePath);
                    string reportPath = "wwwroot/reports/" + project.ProjectName + "_" + project.ApprovalStatus.ToString() + "_" + project.DateOfApproval.ToString("yyyy-mm-dd") + ".docx";
                    string reportFullPath = Path.Combine(_env.ContentRootPath, reportPath);

                    // Create a copy of the template file in the reports folder
                    System.IO.File.Copy(templateFullPath, reportFullPath, true);

                    // Open the copy of the template file
                    using (var doc = WordprocessingDocument.Open(reportFullPath, true))
                    {
                        string approverName = "Candy Orate";

                        // Get the main document part of the docx file
                        MainDocumentPart mainPart = doc.MainDocumentPart;

                        // Find and replace placeholders in the document
                        foreach (var placeholder in mainPart.Document.Descendants<Text>())
                        {
                            if (placeholder.Text.Contains("{{Project}}"))
                            {
                                // Replace the placeholder with the name from the model
                                placeholder.Text = placeholder.Text.Replace("{{Project}}", project.ProjectName);
                            }
                            else if (placeholder.Text.Contains("{{Manager}}"))
                            {
                                // Replace the placeholder with the name from the model
                                placeholder.Text = placeholder.Text.Replace("{{Manager}}", project.Manager);
                            }
                            else if (placeholder.Text.Contains("{{Approver}}"))
                            {
                                // Replace the placeholder with the name from the model
                                placeholder.Text = placeholder.Text.Replace("{{Approver}}", approverName);
                            }
                        }

                        doc.Close();

                        // Convert the document to PDF
                        string pdfPath = Path.ChangeExtension(reportFullPath, ".pdf");

                        // Open the Word application and document
                        Application word = new Application();
                        Document docx = word.Documents.Open(reportFullPath);
                        
                        // Save the document as PDF
                        docx.ExportAsFixedFormat(pdfPath, WdExportFormat.wdExportFormatPDF);
                        docx.Close();                    
                        word.Quit();

                        //Store File path in the Approval Report Path field
                        project.ApprovalReportPath = pdfPath;

                        // send email to notify stakeholders that the project is ready for budget approval
                        var approvalReportPath = new List<string> { "wwwroot/reports/" + project.ProjectName + "_" + project.ApprovalStatus.ToString() + "_" + project.DateOfApproval.ToString("yyyy-mm-dd") + ".pdf" };
                        var recipients = "villabroza.hanssell@gmail.com";
                        var subject = project.ProjectName + " - Responded ";
                        if (project.ApprovalStatus == ApprovalStatusValue.Approved)
                        {
                            var body = $@"
                            <p>Hello {approverName}!</p>
                            <p>You have approved this project. Thank you!</p>
                             ";
                            
                            SendEmail(recipients, subject, body, true, approvalReportPath);
                        }
                        else
                        {
                            var body = $@"
                            <p>Hello {approverName}!</p>
                            <p>You have declined this project. Thank you!</p>
                             ";

                            SendEmail(recipients, subject, body, true, approvalReportPath);
                        }
                     
                    }

                    await _context.SaveChangesAsync();
                    return View("CloseWindow");

                }


            }

            return View();
        }

    }
}
