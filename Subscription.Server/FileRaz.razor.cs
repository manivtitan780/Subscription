using Azure;
using Azure.AI.OpenAI;

using Microsoft.AspNetCore.Components;

using OpenAI.Chat;

namespace Subscription.Server;

public partial class FileRaz : ComponentBase
{
    private string FileContent
    {
        get;
        set;
    } = string.Empty;
        private void CreateToolFunction(ChatCompletionOptions options)
    {
        options.Tools.Add(ChatTool.CreateFunctionTool("extract_candidate",
                                                      "Extract candidate details from resume",
                                                      BinaryData.FromObjectAsJson(new
                                                                                  {
                                                                                      type = "object",
                                                                                      properties = new
                                                                                                   {
                                                                                                       FirstName = new
                                                                                                                   {
                                                                                                                       type = "string", description = "First name of the candidate"
                                                                                                                   },
                                                                                                       LastName = new {type = "string", description = "Last name of the candidate"},
                                                                                                       PhoneNumbers = new
                                                                                                                      {
                                                                                                                          type = "string", description = "Phone number of the candidate"
                                                                                                                      },
                                                                                                       EmailAddresses = new
                                                                                                                        {
                                                                                                                            type = "string", description = "Email address of the candidate"
                                                                                                                        },
                                                                                                       PostalAddress = new
                                                                                                                       {
                                                                                                                           type = "object",
                                                                                                                           properties = new
                                                                                                                                        {
                                                                                                                                            Street = new
                                                                                                                                                     {
                                                                                                                                                         type = "string",
                                                                                                                                                         description = "Street address of the candidate"
                                                                                                                                                     },
                                                                                                                                            City = new
                                                                                                                                                   {
                                                                                                                                                       type = "string",
                                                                                                                                                       description = "City of the candidate"
                                                                                                                                                   },
                                                                                                                                            State = new
                                                                                                                                                    {
                                                                                                                                                        type = "string",
                                                                                                                                                        description = "State of the candidate"
                                                                                                                                                    },
                                                                                                                                            Zip = new
                                                                                                                                                  {
                                                                                                                                                      type = "string",
                                                                                                                                                      description = "Zip code of the candidate"
                                                                                                                                                  }
                                                                                                                                        }
                                                                                                                       },
                                                                                                       CandidateSummary = new
                                                                                                                          {
                                                                                                                              type = "string",
                                                                                                                              description = "Summary of the candidate. Limit to 500 chars only."
                                                                                                                          },
                                                                                                       CandidateKeywords = new
                                                                                                                           {
                                                                                                                               type = "string",
                                                                                                                               description = "Keywords related to the candidate. Limit to 500 chars only."
                                                                                                                           },
                                                                                                       LinkedInProfile = new
                                                                                                                         {
                                                                                                                             type = "string",
                                                                                                                             description = "LinkedIn profile of the candidate"
                                                                                                                         },
                                                                                                       TotalExperienceInMonths =
                                                                                                           new
                                                                                                           {
                                                                                                               type = "integer",
                                                                                                               description = "Total experience of the candidate"
                                                                                                           },
                                                                                                       EducationInfo = new
                                                                                                                       {
                                                                                                                           type = "array",
                                                                                                                           items = new
                                                                                                                                   {
                                                                                                                                       Course = new
                                                                                                                                                   {
                                                                                                                                                       type = "string",
                                                                                                                                                       description = "Course name of the candidate"
                                                                                                                                                   },
                                                                                                                                        School = new
                                                                                                                                                    {
                                                                                                                                                         type = "string",
                                                                                                                                                         description = "School name of the candidate"
                                                                                                                                                    },
                                                                                                                                        Period = new
                                                                                                                                                   {
                                                                                                                                                       type = "string",
                                                                                                                                                       description = "Period of the education"
                                                                                                                                                   },
                                                                                                                                        State = new
                                                                                                                                                   {
                                                                                                                                                       type = "string",
                                                                                                                                                       description = "State of the education"
                                                                                                                                                   },
                                                                                                                                        Country = new
                                                                                                                                                   {
                                                                                                                                                       type = "string",
                                                                                                                                                       description = "Country of the education"
                                                                                                                                                   }
                                                                                                                                   },
                                                                                                                           description = "Education information of the candidate"
                                                                                                                       },
                                                                                                       EmploymentInfo = new
                                                                                                                        {
                                                                                                                            type = "array",
                                                                                                                            items = new
                                                                                                                                    {
                                                                                                                                        Company = new
                                                                                                                                                  {
                                                                                                                                                      type = "string",
                                                                                                                                                      description = "Company name of the candidate"
                                                                                                                                                  },
                                                                                                                                        Start = new
                                                                                                                                                {
                                                                                                                                                    type = "string",
                                                                                                                                                    description = "Start date"
                                                                                                                                                },
                                                                                                                                        End = new
                                                                                                                                              {
                                                                                                                                                  type = "string",
                                                                                                                                                  description = "End date"
                                                                                                                                              },
                                                                                                                                        Location = new
                                                                                                                                                   {
                                                                                                                                                       type = "string",
                                                                                                                                                       description = "Location of the company"
                                                                                                                                                   },
                                                                                                                                        Title = new
                                                                                                                                                {
                                                                                                                                                    type = "string",
                                                                                                                                                    description = "Title of the job profile"
                                                                                                                                                },
                                                                                                                                        Responsibilities = new
                                                                                                                                                      {
                                                                                                                                                          type = "string",
                                                                                                                                                          description = "Responsibilities of the job profile. Confine to 200 chars only."
                                                                                                                                                      }
                                                                                                                                    },
                                                                                                                            description = "Employment information of the candidate",
                                                                                                                            required = (string[]) ["Company", "Start", "End", "Location", "Title", "Responsibilities"]
                                                                                                                        },
                                                                                                       Skills = new
                                                                                                                {
                                                                                                                    type = "array", items = new
                                                                                                                                            {
                                                                                                                                                type = "object",
                                                                                                                                                properties = new
                                                                                                                                                             {
                                                                                                                                                                 Skill = new
                                                                                                                                                                         {
                                                                                                                                                                             type = "string",
                                                                                                                                                                             description =
                                                                                                                                                                                 "Skill of the candidate"
                                                                                                                                                                         },
                                                                                                                                                                 Period = new
                                                                                                                                                                          {
                                                                                                                                                                              type =
                                                                                                                                                                                  "string",
                                                                                                                                                                              description =
                                                                                                                                                                                  "Period worked from to end the skill. If currently working end will be Present."
                                                                                                                                                                          },
                                                                                                                                                                 Month = new
                                                                                                                                                                         {
                                                                                                                                                                             type =
                                                                                                                                                                                 "integer",
                                                                                                                                                                             description =
                                                                                                                                                                                 "Number of Months worked in the skill"
                                                                                                                                                                         }
                                                                                                                                                             },
                                                                                                                                                required = JSONSerializable
                                                                                                                                            },
                                                                                                                    description = "Skills of the candidate"
                                                                                                                }
                                                                                                   }
                                                                                  })));
        ;
    }

    private static readonly string[] JSONSerializable = ["Skill"];

    protected override async Task OnInitializedAsync()
    {
        const string filePath = "C:\\MQT.txt";
        string _fileC = await File.ReadAllTextAsync(filePath);
        string _prompt = "Resume: {0}";
        string _detailedPrompt = string.Format(_prompt, _fileC);
        Uri _endpoint = new("https://maniv-m8swni6e-eastus2.openai.azure.com/");
        AzureKeyCredential _credential = new("4oqUF6EOyTS28yk0RmKhDsvuWh20FnQiVKXtGvNYgrj2fQH2J60SJQQJ99BCACHYHv6XJ3w3AAAAACOG3h9h");
        AzureOpenAIClient _client = new(_endpoint, _credential);

        ChatClient _chatClient = _client.GetChatClient("gpt-4-p"); // This points to o3-mini
        ChatCompletionOptions _chatOptions = new()
                                             {
                                                 Temperature = 0.2f,
                                                 MaxOutputTokenCount = 10000,
                                                 TopP = 0.3f,
                                                 FrequencyPenalty = 0f,
                                                 PresencePenalty = 0f
                                             };
        //_chatOptions.Tools.Add(ChatTool.CreateFunctionTool();
        //CreateToolFunction(_chatOptions);
        //_chatOptions.ToolChoice = ChatToolChoice.CreateFunctionChoice("extract_candidate");
        List<ChatMessage> _messages =
        [
            /*
            new SystemChatMessage("You extract structured resume data as JSON using tools. Strictly use only the fields defined in the function schema. Do not add extra fields or rename fields. Do not combine Start and End into Period."),
            */
            new SystemChatMessage("Parse this resume and return JSON containing: First & Last name, Email Addresses, Phone Numbers, Postal Address (Street, City, State, Zip etc), Education Info (Course, School/College, Period, State, Country), Employment Info (Company, Start, End (enter Present if still working), Months worked, Location, Title of Job, Brief description of ~300 chars), Skills (Skill Name [to be listed individually as candidate might have grouped them together], Period of usage, Months of usage), Candidate Summary (max 1000 chars), Candidate Keywords (max 500 chars), Linkedin Profile, Total Experience in months, and Experience Summary."),
            new UserChatMessage(_detailedPrompt)
        ];
        ChatCompletion _completeChatAsync = await _chatClient.CompleteChatAsync(_messages, _chatOptions);
        FileContent = _completeChatAsync.ToolCalls[0].FunctionArguments.ToString();
    }
}