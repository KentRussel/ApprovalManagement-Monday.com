using ApprovalManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Office.Interop.Word;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office.Word;
using Word = Microsoft.Office.Interop.Word;
using Document = Microsoft.Office.Interop.Word.Document;
using System.IO;
using PdfSharp.Pdf;
using System.IO;
using System.Text.Json.Nodes;
using ApprovalManagement.Controllers;
using System.Text;
using NuGet.Protocol;
using Org.BouncyCastle.Asn1.Ocsp;
using System;

namespace ApprovalManagement.Controllers
{
    public class MondayProjectController : Controller
    {
        private readonly ILogger<MondayProjectController> _logger;

        public MondayProjectController(ILogger<MondayProjectController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        private void SendEmail(string recipient, string subject, string body, bool isBodyHtml = true, List<string> attachmentFilePaths = null)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("dbtird.intern4@gmail.com", "kovibpfwmuefqtil");
            smtpClient.EnableSsl = true;

            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = new MailAddress("dbtird.intern4@gmail.com");
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

        private void SendSecondEmail(string recipient, string subject, string body, bool isBodyHtml = true, List<string> attachmentFilePaths = null)

        {
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("dbtird.intern4@gmail.com", "kovibpfwmuefqtil");
            smtpClient.EnableSsl = true;

            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = new MailAddress("dbtird.intern4@gmail.com");
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

        public async Task<IActionResult> ApprovalForm(string approvalResponse, string approvalstatus, string pulseId, string payload, string approvalstatus2)

        {

            try

            {
                string apiToken = "eyJhbGciOiJIUzI1NiJ9.eyJ0aWQiOjE2NjA2MTI1NSwiYWFpIjoxMSwidWlkIjoxNzM1ODE1NCwiaWFkIjoiMjAyMi0wNi0xN1QwNTo0NjowMC4wMDBaIiwicGVyIjoibWU6d3JpdGUiLCJhY3RpZCI6Njc3NzM2MCwicmduIjoidXNlMSJ9.kHYwChnla-HsIRnc8fvtr1x4O8jp6xFLz-XRPZyYQNg";
                string apiRoot = "https://api.monday.com/v2/";

                using (var client = new MondayClient(apiToken, apiRoot))

                {

                    var _mondayService = new MondayService(client);

                    var itemColumnValue = _mondayService.GetColumnValues(pulseId);
                    var columnValues = itemColumnValue.column_values;
                    var payloadToken = JsonConvert.DeserializeObject<PayloadToken>(payload);

                    if (columnValues != null)

                    {   
                        approvalstatus = columnValues[7].text;
                        approvalstatus2 = columnValues[8].text;
                        approvalResponse = columnValues[9].text;    

                    }

                    ViewData["ApprovalStatus"] = approvalstatus;
                    ViewData["ApprovalResponse"] = approvalResponse;

                    MyViewModel myViewModel = new MyViewModel

                    {
                        DataItems = itemColumnValue,
                        PayloadToken = payloadToken
                    };

                    return View(myViewModel);
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpPost]
        [Route("webhook")]
        public async Task<IActionResult> HandleWebhook()

        {
            var _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");
            var response="Response receieve";
            using (StreamWriter writer = new StreamWriter(_logFilePath, true))

            {
                writer.WriteLine(DateTime.Now.ToString() + ": " + response);
                writer.Close();
            }

            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            var jsonBody = JObject.Parse(requestBody);

            JToken challenge;
            if (jsonBody.TryGetValue("challenge", out challenge))

            {
                response = "Initial request was successful";

                using (StreamWriter writer = new StreamWriter(_logFilePath, true))
                {
                    writer.WriteLine(DateTime.Now.ToString() + ": " + response);
                    writer.Close();
                }
                return Content(requestBody);
                
            }

            else
            {
                string apiToken = "eyJhbGciOiJIUzI1NiJ9.eyJ0aWQiOjE2NjA2MTI1NSwidWlkIjoxNzM1ODE1NCwiaWFkIjoiMjAyMi0wNi0xN1QwNTo0NjowMC4wMDBaIiwicGVyIjoibWU6d3JpdGUiLCJhY3RpZCI6Njc3NzM2MCwicmduIjoidXNlMSJ9.K25dV5_Je9n59GzvI4ec4mNhUw6bh_8ZuikgxzlMMnQ";
                string apiRoot = "https://api.monday.com/v2/";

                using (var client = new MondayClient(apiToken, apiRoot))
                {
                    response = "API Credentials successfully sent";

                    using (StreamWriter writer = new StreamWriter(_logFilePath, true))
                    {
                        writer.WriteLine(DateTime.Now.ToString() + ": " + response);
                        writer.Close();
                    }
                    var _mondayService = new MondayService(client);
                  
                    var payload = jsonBody.GetValue("event").ToString();

                    response = "Event Payload was successful";
                    var otherResponse = payload;

                    using (StreamWriter writer = new StreamWriter(_logFilePath, true))

                    {
                        writer.WriteLine(DateTime.Now.ToString() + ": " + response);
                        writer.WriteLine("Payload: " + otherResponse);
                        writer.Close();
                    }

                    var payloadToken = JsonConvert.DeserializeObject<PayloadToken>(payload);
                    var pulseId = payloadToken.pulseId;

                    var itemColumnValue = _mondayService.GetColumnValues(pulseId);
                    var columnValues = itemColumnValue.column_values;

                    string amountText = columnValues[1].text;
                    string name = columnValues[0].text;
                    string email = columnValues[0].text;
                    string reason = columnValues[2].text;
                    string briefDescription = columnValues[3].text;
                    string approver = columnValues[4].text;
                    string departmentHead = "kentrusselpayumo@gmail.com";//change to dynamic
                    string divisionHead = "kentpayumo@gmail.com";//change to dnamic
                    string ceo = "kentrussel37@gmail.com";//change to dynamic

                    string approverId = columnValues[4].id;

                    var personColumnValues = _mondayService.GetPersonColumnValues(pulseId, approverId);
                    string personId = personColumnValues.id;
                    var approverUserValues = _mondayService.GetUserValues(personId);
                    string approverEmail = approverUserValues.email;


                    // send email to notify Lvl1 
                    var approvalLink = Url.ActionLink("ApprovalForm", "MondayProject", new { projectId = pulseId }, Request.Scheme);
                    var recipients = approverEmail;
                    var subject =  name + " - Budget Approval";
                    var body = $@"
                            <p>Hello {departmentHead}!</p>
                            <p>We are requesting for your approval on our budget request</p> 
                            <p>Please see the details below for your reference.</p><br>
                            <p><b>Reason:</b> {reason}</p>
                            <p><b>Sample Description:</b> {briefDescription}</p>
                            <p><b>Budget:</b> {amountText}</p><br>
                        
                            <!-- Button to open the approval form -->
                            <div style=""display: inline-block;"">
                              <form method='post' action='{approvalLink}'>
                                <input type=""hidden"" name=""pulseId"" value='{pulseId}' />   
                                <input type=""hidden"" name=""payload"" value='{payload}' />
                                <button type='submit' name='approvalResponse' value='Approve' style='background-color: green; height: 30px; width: 100px; color: white;'>Approve</button>
                              </form>
                            </div>

                            <!-- Button to open the decline form -->
                            <div style=""display: inline-block;"">
                              <form method='post' action='{approvalLink}'>
                                <input type=""hidden"" name=""pulseId"" value='{pulseId}' />  
                                <input type=""hidden"" name=""payload"" value='{payload}' /> 
                                <button type='submit' name='approvalResponse' value='Decline' style='background-color: red; height: 30px; width: 100px; color: white;'>Decline</button>
                              </form>
                            </div>
                        ";

                    var approvalLink2 = Url.ActionLink("ApprovalForm", "MondayProject", new { projectId = pulseId }, Request.Scheme);
                    var recipients2 = approverEmail;
                    var subject2 = name + " - Budget Approval";
                    var body2 = $@"
                            <p>Hello {divisionHead}!</p>
                            <p>We are requesting for your approval on our budget request</p> 
                            <p>Please see the details below for your reference.</p><br>
                            <p><b>Reason:</b> {reason}</p>
                            <p><b>Sample Description:</b> {briefDescription}</p>
                            <p><b>Budget:</b> {amountText}</p><br>
                        
                            <!-- Button to open the approval form -->
                            <div style=""display: inline-block;"">
                              <form method='post' action='{approvalLink2}'>
                                <input type=""hidden"" name=""pulseId"" value='{pulseId}' />   
                                <input type=""hidden"" name=""payload"" value='{payload}' />
                                <button type='submit' name='approvalResponse' value='Approve' style='background-color: green; height: 30px; width: 100px; color: white;'>Approve</button>
                              </form>
                            </div>

                            <!-- Button to open the decline form -->
                            <div style=""display: inline-block;"">
                              <form method='post' action='{approvalLink2}'>
                                <input type=""hidden"" name=""pulseId"" value='{pulseId}' />  
                                <input type=""hidden"" name=""payload"" value='{payload}' /> 
                                <button type='submit' name='approvalResponse' value='Decline' style='background-color: red; height: 30px; width: 100px; color: white;'>Decline</button>
                              </form>
                            </div>
                        ";

                    try

                    {
                        SendEmail(recipients, subject, body, true, new List<string>());

                        response = "Email was successfully sent to the Lvl 1 Approver";
                        // Append the log message to the log file
                        using (StreamWriter writer = new StreamWriter(_logFilePath, true))

                        {
                            writer.WriteLine(DateTime.Now.ToString() + ": " + response);
                            writer.Close();
                        }

                    }

                    catch (Exception ex)

                    {
                        // Append the log message to the log file
                        using (StreamWriter writer = new StreamWriter(_logFilePath, true))

                        {
                            writer.WriteLine(DateTime.Now.ToString() + ": " + ex.Message);
                            writer.Close();
                        }
                    }
                    
                    return Ok();
                }
                
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateApprovalStatus(string pulseId, string boardId, string approvalStatusId, string approvalStatus, string remarksId, string remarks, string approvedBudgetId, string approvedBudget, string dateOfApprovalId, string dateOfApproval)
        {
            string apiToken = "eyJhbGciOiJIUzI1NiJ9.eyJ0aWQiOjE2NjA2MTI1NSwidWlkIjoxNzM1ODE1NCwiaWFkIjoiMjAyMi0wNi0xN1QwNTo0NjowMC4wMDBaIiwicGVyIjoibWU6d3JpdGUiLCJhY3RpZCI6Njc3NzM2MCwicmduIjoidXNlMSJ9.K25dV5_Je9n59GzvI4ec4mNhUw6bh_8ZuikgxzlMMnQ";
            string apiRoot = "https://api.monday.com/v2/";

            using (var client = new MondayClient(apiToken, apiRoot))
            {
                var _mondayService = new MondayService(client);

                var itemColumnValue = _mondayService.GetColumnValues(pulseId);
                var columnValues = itemColumnValue.column_values;
                
                _mondayService.Change_Multiple_Column_Value(pulseId, boardId, approvalStatusId, approvalStatus, remarksId, remarks, approvedBudgetId, approvedBudget, dateOfApprovalId, dateOfApproval, out string err);

                return View("CloseWindow");
            }
            return View();
        }


    }
}
