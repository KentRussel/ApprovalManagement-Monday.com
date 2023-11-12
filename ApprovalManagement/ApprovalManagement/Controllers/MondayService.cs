
using Microsoft.Ajax.Utilities;
using ApprovalManagement.Helper;
using ApprovalManagement.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NuGet.Common;
using System.Collections;
using Microsoft.CodeAnalysis;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Security.Policy;

namespace ApprovalManagement.Controllers
{
    public class MondayService
    {
        private MondayClient _mondayClient;

        public MondayService(MondayClient client)

        {
            _mondayClient = client;
        }
        public string GetResponseData(string query)
        {
            //query = query.Normalize();
            StringContent content = new StringContent(query, Encoding.UTF8, "application/json");
            using (var response = _mondayClient.PostAsync("", content))
            {
                
                if (!response.Result.IsSuccessStatusCode)
                {
                    throw new Newtonsoft.Json.JsonException($"The response from {_mondayClient.BaseAddress} was {response.Result.StatusCode}");
                }
                return response.Result.Content.ReadAsStringAsync().Result;
            }
        }

        public DataItems GetColumnValues(string pulseID)

        {
            // Query for retriving the json value of the entire key-value of the columns
            string query = "{ \"query\": \"" + @"{ items (ids: " + pulseID + ") " +
                "{ name column_values (ids:[long_text, item_id, timeline, numbers9, numeric," +
                "status, status9, person, people, date4, long_text7, date, files]) " +
                "{ id text title value } } }" + "\" }";
            string response = GetResponseData(query);

            var responseBody = JObject.Parse(response);
            string itemValue = responseBody["data"]["items"][0].ToString();
            var itemColumnValue = JsonConvert.DeserializeObject<DataItems>(itemValue);

            return itemColumnValue;
        }
        private dynamic ParseGraphQLResponse(string responseString, string collectionKey = "")

        {
            JObject responseObject = JObject.Parse(responseString);


            // The 'errors' and 'data' keys are part of the standard response format set in the GraphQL spec (see https://graphql.github.io/graphql-spec/draft/#sec-Response)
            if (responseObject["errors"] != null)
            {
                throw new Newtonsoft.Json.JsonException($"The request was successful but contained errors: " +
                                       $"{JsonConvert.DeserializeObject<dynamic>(responseObject["errors"].ToString())}");
            }
            if (responseObject["data"] == null)
            {
                throw new Newtonsoft.Json.JsonException("The request was successful but contained no data");
            }

            dynamic data;
            data = collectionKey == "" ? JsonConvert.DeserializeObject<dynamic>(responseObject["data"].ToString()) :
                                         JsonConvert.DeserializeObject<dynamic>(responseObject["data"][collectionKey].ToString());

            return data;
        }
        public List<Item> GetItemDetails(string pulseID)

        {
            string query = "{ \"query\": \"" + @"{ items (ids: " + pulseID + ") " +
                        "{ name column_values (ids:[long_text, item_id, timeline, numbers9, " +
                        "status, status9, person, people, date4, long_text7, date, files]) " +
                        "{ id text title value } } }" + "\" }";
            string response = GetResponseData(query);

            dynamic data = ParseGraphQLResponse(response, "items");

            var json = data == null ? "" : data.ToString();

            dynamic a = JsonConvert.DeserializeObject(json);

            var jsonOptions = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            List<Item> itemDetails = JsonConvert.DeserializeObject<List<Item>>(data.ToString());

            return itemDetails;
        }

        public PersonValue GetPersonColumnValues(string pulseID, string peopleId)

        {
            string query = "{ \"query\": \"" + @"{ items (ids: " + pulseID + ") {" +
                        "column_values (ids: " + peopleId + ") {" +
                        "text value } } }" + "\" }";

            string response = GetResponseData(query);
            var responseBody = JObject.Parse(response);
            string itemValue = responseBody["data"]["items"][0].ToString();
            var itemColumnValue = JsonConvert.DeserializeObject<DataItems>(itemValue);
            string personValue = itemColumnValue.column_values[0].value.ToString();
            var personJSON = JObject.Parse(personValue);
            string personResponseBody = personJSON["personsAndTeams"][0].ToString();
            var personColumnValue = JsonConvert.DeserializeObject<PersonValue>(personResponseBody);

            return personColumnValue;
        }

        public UserValue GetUserValues(string personColumnId)
        {
            string query = "{ \"query\": \"" + @"{ users (ids: " + personColumnId + ") {" +
                        "name email url } }" + "\" }";

            string response = GetResponseData(query);
            var responseBody = JObject.Parse(response);
            string itemValue = responseBody["data"]["users"][0].ToString();
            var itemColumnValue = JsonConvert.DeserializeObject<UserValue>(itemValue);;

            return itemColumnValue;
        }

        public List<Item> GetItemDetails(string pulseID, string colName)
        {
            string query = "{ \"query\": \"" + @"{ items (ids: " + pulseID + ") { name column_values (ids:[colName]) { id text title value } } }" + "\" }";

            query = query.Replace("colName", colName);

            string response = GetResponseData(query);

            dynamic data = ParseGraphQLResponse(response, "items");

            var json = data == null ? "" : data.ToString();

            dynamic a = JsonConvert.DeserializeObject(json);

            var jsonOptions = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            List<Item> itemDetails = JsonConvert.DeserializeObject<List<Item>>(data.ToString());

            return itemDetails;
        }
        public List<Item> GetParentDetails(string parentID)
        {
            string query = "{ \"query\": \"" + @"{ items (ids: " + parentID + ") { name column_values (ids:[person, date, connect_boards]) { id text title value } } }" + "\" }";
            string response = GetResponseData(query);

            dynamic data = ParseGraphQLResponse(response, "items");

            var json = data == null ? "" : data.ToString();

            dynamic a = JsonConvert.DeserializeObject(json);

            var jsonOptions = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            List<Item> itemDetails = JsonConvert.DeserializeObject<List<Item>>(data.ToString());

            return itemDetails;
        }
        public bool Update_Date(string subID, string boardID, string date, string parent, string child, out string err)
        {
            string query = @"{""query"" : ""mutation { change_multiple_column_values (board_id: zzzBoardID, item_id: zzzItemID , column_values: \""{\\\""zzzDate0\\\"" : {\\\""zzzDate\\\"" : \\\""zzzDT\\\""}}\""){ id} }""}";

            query += "";
            //Date
            query = query.Replace("zzzBoardID", boardID);
            query = query.Replace("zzzItemID", subID);
            query = query.Replace("zzzDate0", child);
            query = query.Replace("zzzDate", "date");
            query = query.Replace("zzzDT", date);

            string response = GetResponseData(query);


            err = response;
            return response.ToLower().Contains("error") ? false : true;
        }

        public bool Update_People(string subID, string boardID, string assignee, string parent, string child, out string err)
        {
            string query = @"{""query"" : ""mutation { change_multiple_column_values (board_id: zzzBoardID, item_id: zzzItemID , column_values: \""{\\\""zzzA\\\"" : {\\\""zzzB\\\"" : [{\\\""zzzC\\\"" : \\\""zzzD\\\"", \\\""zzzE\\\"" : \\\""zzzF\\\""}]}}\""){ id} }""}";

            query = query.Replace("zzzBoardID", boardID);
            query = query.Replace("zzzItemID", subID);

            //Assignee
            query = query.Replace("zzzA", child);
            query = query.Replace("zzzB", "personsAndTeams");
            query = query.Replace("zzzC", "id");
            query = query.Replace("zzzD", assignee);
            query = query.Replace("zzzE", "kind");
            query = query.Replace("zzzF", "person");


            string response = GetResponseData(query);


            err = response;
            return response.ToLower().Contains("error") ? false : true;
        }
        public bool Update_Status(string subID, string boardID, string status, string parent, string child, out string err)
        {
            string query = @"{""query"" : ""mutation { change_multiple_column_values (board_id: zzzBoardID, item_id: zzzItemID , create_labels_if_missing: true, column_values: \""{\\\""zzzStatus\\\"" : {\\\""label\\\"" : \\\""zStatus\\\""}}\""){ id} }""}";

            query = query.Replace("zzzBoardID", boardID);
            query = query.Replace("zzzItemID", subID);

            //status
            query = query.Replace("zzzStatus", child);
            query = query.Replace("label", "label");
            query = query.Replace("zStatus", status);


            string response = GetResponseData(query);

            err = response;
            return response.ToLower().Contains("error") ? false : true;
        }
        /// <summary>
        /// Get pulseID details
        /// </summary>
        public List<Item> GetSubitems(string itemID)
        {
            string query = "{ \"query\": \"" + @"{ items (ids: " + itemID + ") { name column_values { id text title value } } }" + "\" }";
            string response = GetResponseData(query);

            dynamic data = ParseGraphQLResponse(response, "items");

            var json = data == null ? "" : data.ToString();

            dynamic a = JsonConvert.DeserializeObject(json);

            var jsonOptions = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            List<Item> itemDetails = JsonConvert.DeserializeObject<List<Item>>(data.ToString());

            return itemDetails;
        }

        //CHANGE MULTIPLE COLUMN VALUE
        public bool Change_Multiple_Column_Value(string subID, string boardID, string statusColumnId, string statusValue, string textColumnId, string textValue, string otherTextColumnId, string otherTextValue, string otherTextColumnId1, string otherTextValue1, out string err)
        {
            string query = @"{""query"" : ""mutation { change_multiple_column_values (board_id: zzzBoardID, item_id: zzzItemID , create_labels_if_missing: true, column_values: \""{\\\""statusColumn\\\"" : {\\\""zlabel\\\"" : \\\""statusValue\\\""}, \\\""textColumn\\\"" : \\\""textValue\\\"", \\\""otherTextColumn\\\"" : \\\""otherTextValue\\\"", \\\""datetimeTextColumn1\\\"" : \\\""datetimeTextValue1\\\""}\""){ id} }""}";
            query = query.Replace("zzzBoardID", boardID);
            query = query.Replace("zzzItemID", subID);

            //Status Column
            query = query.Replace("statusColumn", statusColumnId);
            query = query.Replace("zlabel", "label");
            query = query.Replace("statusValue", statusValue);

            //Text Column
            query = query.Replace("textColumn", textColumnId);
            query = query.Replace("textValue", textValue);

            //Other Text Column
            query = query.Replace("otherTextColumn", otherTextColumnId);
            query = query.Replace("otherTextValue", otherTextValue);

            query = query.Replace("datetimeTextColumn1", otherTextColumnId1);
            query = query.Replace("datetimeTextValue1", otherTextValue1);

            string response = GetResponseData(query);
            err = response;
            return err.ToLower().Contains("error") ? false : true;


        }

        //CHANGE SIMPLE COLUMN VALUE
        public bool Update_Simple_Column_Value(string subID, string boardID,string columnId, string value, string dataType, out string err)
        {
            //string query = @"{""query"" : ""mutation { change_column_value (board_id: zzzBoardID, item_id: zzzItemID , 
            //                column_id: ""zColumn"", value: ""{\""zlabel\"": \""zValue\""}"", create_labels_if_missing: true){ id} }}""}";

            string query = @"{""query"" : ""mutation { change_multiple_column_values (board_id: zzzBoardID, item_id: zzzItemID , create_labels_if_missing: true, column_values: \""{\\\""zColumn\\\"" : {\\\""zlabel\\\"" : \\\""zValue\\\""}}\""){ id} }""}";

            query = query.Replace("zzzBoardID", boardID);
            query = query.Replace("zzzItemID", subID);

            //Column
            query = query.Replace("zColumn", columnId);
            query = query.Replace("zlabel", dataType);

            query = query.Replace("zValue", value);

            string response = GetResponseData(query);

            err = response;
            return err.ToLower().Contains("error") ? false : true;


        }
        public bool AlertMessage(string msg, string pulseId, string pulseBoard, string pulseCol)
        {
            string query = @"{""query"" : ""mutation { change_multiple_column_values (board_id: zzzBoardID, item_id: zzzId , create_labels_if_missing: true, column_values: \""{\\\""zzzTeam\\\"" : {\\\""label\\\"" : \\\""zteam\\\""},\\\""zzzAlert\\\"" : \\\""zzzText\\\""}\""){ id} }""}";

            //Recurring status
            var stats = msg.ToLower().Contains("error") ? "Error" : "Done";
            query = query.Replace("zzzTeam", "status");
            query = query.Replace("label", "label");
            query = query.Replace("zteam", stats);

            //Alert Message
            query = query.Replace("zzzAlert", "text");
            query = query.Replace("zzzText", msg);

            query = query.Replace("zzzBoardID", pulseBoard);

            query = query.Replace("zzzId", pulseId);

            string response = GetResponseData(query);
            return response.ToLower().Contains("error") ? false : true;
        }
        public List<Item> GetFreqDetails(string freqID)
        {
            string query = "{ \"query\": \"" + @"{ items (ids: " + freqID + ") { name column_values { id text title value } } }" + "\" }";
            string response = GetResponseData(query);
            dynamic data = ParseGraphQLResponse(response, "items");

            var json = data == null ? "" : data.ToString();

            dynamic a = JsonConvert.DeserializeObject(json);

            var jsonOptions = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            List<Item> itemDetails = JsonConvert.DeserializeObject<List<Item>>(data.ToString());

            return itemDetails;
        }

        //MONDAY EVENT
        public bool CreateTask(string name, string boardId, string itemId, string targetdate, out string err)
        {
            string query = @"{""query"" : ""mutation { create_subitem (parent_item_id: zzzItemID, item_name: \""zzzName\"" , create_labels_if_missing: true, column_values: \""{\\\""zzzDate0\\\"" : {\\\""zzzDate\\\"" : \\\""zzzDT\\\""}}\""){ id} }""}";

            //Due Date
            query = query.Replace("zzzItemID", itemId);
            query = query.Replace("zzzName", name);
            query = query.Replace("zzzDate0", "date0");
            query = query.Replace("zzzDate", "date");
            query = query.Replace("zzzDT", targetdate);

            string response = GetResponseData(query);

            err = response;
            return response.ToLower().Contains("error") ? false : true;
        }
    }
}