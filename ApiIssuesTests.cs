using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;

namespace RestSharpIssuesProject
{
    public class ApiIssuesTests
    {
        private RestClient client;
        private const string baseURL = "https://api.github.com";
        private const string username = "Irida0";
        private const string password = "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";

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(baseURL);
            client.Authenticator = new HttpBasicAuthenticator(username, password);
        }

        [Test]
        public void Test_GetAllIssuesForRepo()
        {
            var request = new RestRequest("/repos/Irida0/RestSharpApiTests/issues", Method.Get);
            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status property:");
            var issues = JsonSerializer.Deserialize<List<Issue>>(response.Content);
            foreach (var issue in issues) 
            {
                Assert.Greater(issue.id, 0);  
                Assert.Greater(issue.number, 0);  
                Assert.IsNotEmpty(issue.title);  
            }
        }

        [Test]
        public void Test_GetIssuesByValidNumber()
        {
            var request = new RestRequest("/repos/Irida0/RestSharpApiTests/issues/1", Method.Get);
            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status property:");
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);
            Assert.Greater(issue.id, 0);
            Assert.Greater(issue.number, 0);           
        }

        [Test]
        public void Test_GetIssuesByInalidNumber()
        {
            var request = new RestRequest("/repos/Irida0/RestSharpApiTests/issues/-1", Method.Get);
            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Status property:");            
        }

        [Test]
        public void Test_CreateNewIssueWithValidDataAndAutho()
        {
            var request = new RestRequest("/repos/Irida0/RestSharpApiTests/issues", Method.Post);
            var newIssueTitle = TestContext.CurrentContext.Random.GetString(20);

            var issueBody = new
            {
                title = newIssueTitle
            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status property:");

            var issue = JsonSerializer.Deserialize<Issue>(response.Content);
            Assert.GreaterOrEqual(issue.id, 0);
            Assert.Greater(issue.number, 0);
            Assert.AreEqual(issue.title, issueBody.title);
        }

        [Test]
        public void Test_CreateNewIssueWithoutAutho()
        {
            //No Autho
            client.Authenticator = null;
            var request = new RestRequest("/repos/Irida0/RestSharpApiTests/issues", Method.Post);
            var newIssueTitle = TestContext.CurrentContext.Random.GetString(20);

            var issueBody = new
            {
                title = newIssueTitle
            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Status property:");
        }

        [Test]
        public void Test_CreateNewIssueWithoutTitleWithAutho()
        {
            var request = new RestRequest("/repos/Irida0/RestSharpApiTests/issues", Method.Post);
            var newIssueTitle = TestContext.CurrentContext.Random.GetString(20);

            var issueBody = new
            {
                //missing title here
            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity), "Status property:");
        }

        [Test]
        public void Test_EditValidIssueById()
        {
            var request = new RestRequest("/repos/Irida0/RestSharpApiTests/issues/1", Method.Patch);
            var newIssueTitle = TestContext.CurrentContext.Random.GetString(20);

            var issueBody = new
            {
                title = newIssueTitle
            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status property:");

            var issue = JsonSerializer.Deserialize<Issue>(response.Content);
            Assert.GreaterOrEqual(issue.id, 0);
            Assert.Greater(issue.number, 0);
            Assert.AreEqual(issue.title, issueBody.title);
        }

        [Test]
        public void Test_EditInvalidIssueById()
        {
            var request = new RestRequest("/repos/Irida0/RestSharpApiTests/issues/-1", Method.Patch);
            var newIssueTitle = TestContext.CurrentContext.Random.GetString(20);

            var issueBody = new
            {
                title = newIssueTitle
            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Status Property:");       
        }

        [Test]
        public void Test_EditValidIssueByIdWithoutAutho()
        {
            //No Autho
            client.Authenticator = null;
            var request = new RestRequest("/repos/Irida0/RestSharpApiTests/issues/1", Method.Patch);
            var newIssueTitle = TestContext.CurrentContext.Random.GetString(20);

            var issueBody = new
            {
                title = newIssueTitle
            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Status Property:");
        }
    }
}