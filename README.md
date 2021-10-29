# BizTalkOrchestrationTester
With BizTalk Orchestration Tester you can unit test an orchestration without deploying it

You can setup a post build action to generate the C# code of an orchestration and then you can add Unit Tests to test the orchestration. In case ports or the interface changes the new code generated may break the unit tests that need to be fixed and rerun.


Start the orchestration calling the Call method... hook on the send and receive ports with function delegates so you can control the messages the orchestraiton sends and you can return any kind of XML repsonse back to the orchestration (including an exception)
