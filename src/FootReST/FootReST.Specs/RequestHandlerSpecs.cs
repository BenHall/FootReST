using Xunit;

namespace FootReST.Specs
{
    public class RequestHandlerSpecs_Responses
    {
        RequestHandler handler;

        public RequestHandlerSpecs_Responses()
        {
            handler = new RequestHandler(null);
            handler.DefineCustomResponse("GET", "endpoint", "response");            
        }      

        [Fact]
        public void Defining_custom_response_is_stored_in_db()
        {
            string response = handler.GetResponseForEndpoint("GET", "endpoint");

            Assert.Equal("response", response);
        }

        [Fact]
        public void GetRequestedEndpoint_returns_empty_string_for_post()
        {
            string response = handler.GetResponseForEndpoint("POST", "endpoint");
            Assert.True(string.IsNullOrEmpty(response));
        }

        [Fact]
        public void Define_different_responses_based_on_verb()
        {
            handler.DefineCustomResponse("POST", "endpoint", "post response");    

            string getResponse = handler.GetResponseForEndpoint("GET", "endpoint");
            string postResponse = handler.GetResponseForEndpoint("POST", "endpoint");

            Assert.Equal("response", getResponse);
            Assert.Equal("post response", postResponse);
        }
    }

    public class RequestHandlerSpecs_GetVerb
    {
        RequestHandler handler;

        public RequestHandlerSpecs_GetVerb()
        {
            handler = new RequestHandler(null);
            handler.DefineCustomResponse("GET", "endpoint", "response");
        }

        [Fact]
        public void GetVerb_returns_GET_for_Get_request()
        {
            string verb = handler.GetRequestVerb("GET REQUEST");
            Assert.Equal("GET", verb);
        }
    }

    public class RequestHandlerSpecs_WildcardEndpoints
    {
        RequestHandler handler;

        public RequestHandlerSpecs_WildcardEndpoints()
        {
            handler = new RequestHandler(null);
            handler.DefineCustomResponse("GET", "end(.*?)", "response");            
        }      

        [Fact]
        public void When_requesting_wildcard_endpoints_it_makes_on_any_part()
        {
            string response = handler.GetWildcardResponse("GET", "endpoint");
            Assert.Equal("response", response);
        }
    }
}
