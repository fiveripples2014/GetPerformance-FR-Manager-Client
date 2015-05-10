<h1>Read Me</h1>

<h3>Note<h3>
<p>This is the client side application, which is a windows service. So you can't directly run it from Visual Studio. First you have to install the windows service to your machine to test it.<p>

<h3>Instructions</h3>

<ul>
	<li>The "FRManagerClient.exe" can be found in "FRManagerClient\FRManagerClient\bin\Debug"</li>

	<li>Go to "Start" >> "All Programs" >> "Microsoft Visual Studio 2013" >> "Visual Studio Tools" then click "Developer Command Prompt for VS2013".</li>

	<li>Navigate to the place where you can find "FRManagerClient.exe" using "cd" command</li>

	<li>Then use "InstallUtil.exe FRManagerClient.exe” to install it to your machine</li>

	<li>Go to your services in the machine to find the "FRManagerClientService"</li>

	<li>As the service is set to Start Type "Manual", you have to manually start it by yourself by right click "Start"</li>

	<li>Now the service is running, and you will hear the speech synthesizer if you have turned on the speakers</li>

	<li>A "DataLogFile.json" will be created in the directory where the "FRManagerClient.exe" file can be found. It will have some of the data you need from your computer as a JSON object</li>

	<li>Note the time is in UTC for the remote usageand time convertions</li>

	<li>Uninstall the service, use the command InstallUtil.exe /u “FRManagerClient.exe”</li>
</ul>


<h3>Problems</h3>

<ul>

<li>Getting the user name of the current logged in user</li>

<li>Need to create dynamic object fields and field names to get some information related to disk and memory</li>

</ul>

<h3>Last Update</h3>
<p>10 May 2015 1.12 p.m by Hasitha Prageeth</p>