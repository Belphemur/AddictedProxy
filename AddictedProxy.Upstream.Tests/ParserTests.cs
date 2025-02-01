using System.Text;
using AddictedProxy.Culture.Service;
using AddictedProxy.Upstream.Service;
using AngleSharp.Html.Parser;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AddictedProxy.Upstream.Tests;

public class ParserTests
{
    private Parser _parser;

    [SetUp]
    public void Setup()
    {
        var logger = Substitute.For<ILogger<Parser>>();
        var cultureParser = Substitute.For<ICultureParser>();
        cultureParser.FromStringAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(new Culture.Model.Culture("English", "en", "en"));
        _parser = new Parser(new HtmlParser(), logger, cultureParser);
    }

    [Test]
    public async Task Test_Success_Download_Usage()
    {
        #region testData

        var html = @" 
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>

<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
<title>Addic7ed.com - For all those TV Series Addic7s: Subtitles, Tv Series and Movies Talk, Forum and more -</title>
<link href=""css/wikisubtitles.css"" rel=""stylesheet"" title=""default"" type=""text/css"" />
<link rel=""SHORTCUT ICON"" href=""favicon.ico"" /> 
<link rel=""stylesheet"" type=""text/css"" href=""css/panel.css"" >
</head>
<body>


<center>
<br />
<table border=""0"">
<tr>
  <td rowspan=""9""><a href=""/""><img width=""350"" height=""111"" src=""https://www.addic7ed.com/images/addic7edlogonew.png""  border=""0""  title=""Addic7ed.com - Quality Subtitles for TV Shows and movies"" alt=""Addic7ed.com - Quality Subtitles for TV Shows and movies"" /></a></td>
</tr>
<tr><td align=""center"" colspan=""2"">
<h1><small>Download free subtitles for TV Shows and Movies.</small>&nbsp; 
<select name=""applang"" class=""inputCool"" onchange=""changeAppLang();"" id=""comboLang""><option value=""ar"">Arabic</option><option value=""ca"">Catala</option><option selected=""selected"" value=""en"">English</option><option value=""eu"">Euskera</option><option value=""fr"">French</option><option value=""ga"">Galician</option><option value=""de"">German</option><option value=""gr"">Greek</option><option value=""hu"">Hungarian</option><option value=""it"">Italian</option><option value=""fa"">Persian</option><option value=""pl"">Polish</option><option value=""pt"">Portuguese</option><option value=""br"">Portuguese (Brazilian)</option><option value=""ro"">Romanian</option><option value=""ru"">Russian</option><option value=""es"">Spanish</option><option value=""se"">Swedish</option></select></h1>
</td></tr>
<tr><td align=""center"" colspan=""2"">

<script language=""javascript"">
var url=""/msgspopup.php?count=1"";		editwin = window.open(url, ""msgswin"", 'height=200,width=350,toolbar=0,location=0,statusbar=0,menubar=0'); 
		if (editwin.focus) {editwin.focus()}
</script>
<div id=""hBar"">
			  <ul>
				<li><a class=""button white"" href=""/panel.php"">My Profile</a></li><li><a class=""button white"" title=""For nonexistent episodes ONLY. For new versions, please use the 'Upload a new version' button on episodes pages."" href=""/newsub.php"">Upload</a></li>			<li><a class=""button white"" href=""/shows.php"">Shows</a></li>
				<li><a class=""button white"" href=""http://www.sub-talk.net/topic/6961-recruitment-syncers-only-not-translators/"">Join the team</a></li>
				<li><a class=""button white"" href=""http://www.sub-talk.net"">Forum</a></li>
				<li><a class=""button white"" href=""/logout.php"">Logout</a></li>
			  </ul>
			  </div>
</td></tr> 

<tr>
  <td>
</td><td>
	
    <a href=""http://twitter.com/addic7ed"" target=""_blank""><img width=""32"" height=""32"" src=""https://www.addic7ed.com/images/twitter_right.png"" alt=""Twitter"" border=""0"" /></a>
	<a href=""irc://irc.efnet.net:6667/addic7ed""><img width=""32"" height=""32"" src=""https://www.addic7ed.com/images/irc-right.png"" alt=""IRC"" border=""0"" /></a>


  </td>
 </tr>
 <tr>
   <td><a href=""https://www.patreon.com/bePatron?u=7679598""><img width=""170px"" height=""40px"" src=""https://c5.patreon.com/external/logo/become_a_patron_button.png"" /></a></td><td><iframe src=""https://www.facebook.com/plugins/follow.php?href=https%3A%2F%2Fwww.facebook.com%2Faddic7ed&width=63&height=65&layout=box_count&size=small&show_faces=true&appId=118151234893132"" width=""63"" height=""65"" style=""border:none;overflow:hidden"" scrolling=""no"" frameborder=""0"" allowTransparency=""true""></iframe>
    </td>
<td>       

         <div alt=""Donation Status"" class=""c100 p0 small"">
                    <span><a title=""Donation Status"" href=""https://www.addic7ed.com/contact.php"">0%</a></span>
                    <div class=""slice"">
                        <div class=""bar""></div>
                        <div class=""fill""></div>
                    </div>
                </div></td>
 </tr>

</table>
</center>

<center>

<!--[if lt IE 7]>
 <style type=""text/css"">
 div, img { behavior: url(https://www.addic7ed.com/js/iepngfix.htc) }
 </style>
<![endif]-->


<table width=""94%"" border=""0"" cellpadding=""0"" cellspacing=""0"">
  <tr bgcolor=""#FFFFFF"">
    <th width=""14%"" height=""37"" bgcolor=""#009BCA"" scope=""row""></th>
    
    <td width=""83%"" height=""37""><center>
<div id=""container""> 
    	<table class=""tabel"" border=""0"">
        	<tr> <!-- table header -->
            	<td class=""tablecorner""><img src=""images/tl.gif"" /></td>
                <td></td>
                <td class=""tablecorner""><img src=""images/tr.gif"" /></td>
            </tr>
            <tr>
            	<td></td>
                <td><div class=""subItem""><big><center><a href=""https://www.addic7ed.com/user/1046727""><font color=""blue"">Jouno</font></a> - </center></div>
</td>
                <td></td>
            </tr>
            <tr> <!-- table footer -->
            	<td class=""tablecorner""><img src=""images/bl.gif"" /></td>
                <td></td>
                <td class=""tablecorner""><img src=""images/br.gif"" /></td>
            </tr>
        </table>
    </div></td>
  </tr>
<tr><td><img src=""images/invisible.gif""></td></tr>
  <tr>
<th height=""58"" bgcolor=""#009BCA"" scope=""row""><div class=""navbar"">
<!-- *********************************Start Menu****************************** -->
<div class=""mainDiv"" >
<div class=""topItem"" >Profile</div>
<div class=""dropMenu"" ><!-- -->
        <div class=""subMenu"" style=""display:inline;"">
                <div class=""subItem""><a href=""/panel_edit_profile.php"">Edit Profile</a></div>
                <div class=""subItem""><a href=""/mydownloads.php"">My Downloads</a></div>
                <div class=""subItem""><a href=""/following.php"">I'm Following</a></div>
                <div class=""subItem""><a href=""/myviews.php"">I've Viewed</a></div>
        </div>
</div>
</div>
<!-- *********************************End Menu****************************** --></th>
    <td bgcolor=""#009BCA"" align=""center"">
<div id=""container""> 
    	<table class=""tabel70"" border=0"">
        	<tr> <!-- table header -->
            	<td class=""tablecorner""><img src=""/images/tl.gif"" /></td>
                <td></td>
                <td class=""tablecorner""><img src=""/images/tr.gif"" /></td>
            </tr>
            <tr>
            	<td></td>
                <td>
<table class=""tabel"" border=""0"" align=""center"">
      <tr>
        <td align=""center"" rowspan=""15""><img src=""https://www.gravatar.com/avatar.php?gravatar_id=187b22576392e7ecbceea5232b45bdde&size=100&rating=X&border=FF0000"" width=""100"" height=""100"" /></td>
        <td></td>
      </tr>
      <tr>
        <td class=""topItem"">Public Profile</td>
        <td><a href=""https://www.addic7ed.com/user/1046727""><font color=""blue"">Jouno</font></a</td>
        <td></td>
      </tr>
      <tr>
        <td class=""topItem"">Addict Since</td>
        <td>2022-04-03 02:51:21</td>
        <td></td>
      </tr>
      <tr>
        <td class=""topItem"">Web site</td>
        <td></td>
        <td></td>
      </tr>
      <tr>
        <td class=""topItem"">Signature</td>
        <td></td>
        <td></td>
      </tr>
      <tr>
        <td class=""topItem"">Gender</td>
        <td>Non-Binary</td>
        <td></td>
      </tr>
      <tr>
        <td class=""topItem"">Downloads by User Today</td>
        <td><a href='mydownloads.php'>12 of 40</a></td>
        <td></td>
      </tr>
      <tr>
        <td class=""topItem"">Downloads from IP Today</td>
        <td><a href='log.php?mode=ip&ip=70.81.204.140'>3</a></td>
        <td></td>
      </tr>
      <tr>
        <td class=""topItem"">Class</td>
        	<td>Member</td>
        <td></td>
      </tr>
      <tr>
        <td class=""topItem"">Last Seen</td>
        <td>2022-12-13 14:28:26</td>
        <td></td>
      </tr>
       <tr>
        <td class=""topItem"">Last IP</td>
        <td><a href='log.php?mode=ip&ip=70.81.204.140'>70.81.204.140</a></td>
        <td></td>
      </tr>
            <tr>
        <td class=""topItem"">Episodes created</td>
        <td>0</td>
        <td></td>
      </tr>
    </table>
</td>
                <td></td>
            </tr>
            <tr> <!-- table footer -->
            	<td class=""tablecorner""><img src=""/images/bl.gif"" /></td>
                <td></td>
                <td class=""tablecorner""><img src=""/images/br.gif"" /></td>
            </tr>
        </table>
    </div>
</td>
             
<tr bgcolor=""#009BCA"">
    <th height=""80"" scope=""row""><div class=""navbar"">
<!-- *********************************Start Menu****************************** -->
<div class=""mainDiv"" >
<div class=""topItem"" >Messages</div>        
<div class=""dropMenu"" ><!-- -->
	<div class=""subMenu"" style=""display:inline;"">
		<div class=""subItem""><a href=""msginbox.php"">Inbox</a></div>
	        <div class=""subItem""><a href=""msgoutbox.php"">Outbox</a></div>
		<div class=""subItem""><a href=""msg-create.php"">Compose</a></div>
	</div>
</div>
</div>
<!-- *********************************End Menu****************************** --></th>
    <td colspan=""2"" bgcolor=""#009BCA""></td>
  </tr>
  <tr bgcolor=""#009BCA"">
    <th height=""74"" bgcolor=""#009BCA"" scope=""row""><div class=""navbar"">
<!-- *********************************Start Menu****************************** -->
<div class=""mainDiv"" >
<div class=""topItem"" >RSS Feeds</div>        
<div class=""dropMenu"" ><!-- -->
	<div class=""subMenu"" style=""display:inline;"">
		<div class=""subItem""><a href=""/log.php?mode=news"">Latest News</a></div>
		<div class=""subItem""><a href=""/rss.php?mode=hotspot"">New Releases</a></div>
	    <div class=""subItem""><a href=""/rss.php?mode=completed"">Latest Files</a></div>
		<div class=""subItem""><a href=""/rss.php?mode=edited"">Latest Edited Files</a></div>
		<div class=""subItem""><a href=""/rss.php?mode=translated"">Latest Started Translations</a></div>
		<div class=""subItem""><a href=""/rss.php?mode=versions"">Latest New Versions</a></div>
	</div>
</div>
</div>
<!-- *********************************End Menu****************************** --></th>
    <td colspan=""2"" rowspan=""5""></td>
  </tr>
  <tr bgcolor=""#FFFFFF"">
    <th height=""75"" bgcolor=""#009BCA"" scope=""row""><div class=""navbar"">
  <!-- *********************************Start Menu****************************** -->
  <div class=""mainDiv"" >
  <div class=""topItem"" >Support Addic7ed</div>        
  <div class=""dropMenu"" ><!-- -->
    <div class=""subMenu"" style=""display:inline;"">
      <div class=""subItem""><a href=""/contact.php"">Donations</a></div>
      </div>
  </div>
  </div>
  <!-- *********************************End Menu****************************** --></th>
  </tr>
  <tr bgcolor=""#009BCA"">
    <th height=""70"" scope=""row""><div class=""navbar"">
  <!-- *********************************Start Menu****************************** -->
  <div class=""mainDiv"" >
  <div class=""topItem"" >Help & Contact</div>        
  <div class=""dropMenu"" ><!-- -->
    <div class=""subMenu"" style=""display:inline;"">
      <div class=""subItem""><a href=""http://www.sub-talk.net/"">Forums</a></div>
      <div class=""subItem""><a href=""/contact.php"">Contact</a></div>      
      </div>
  </div>
  </div>
  <!-- *********************************End Menu****************************** --></th>
  </tr>
  <tr bgcolor=""#009BCA"">
    <th height=""98"" scope=""row""><div class=""navbar"">
  <!-- *********************************Start Menu****************************** -->
  <div class=""mainDiv"" >
  <div class=""topItem"" >Miscellaneous</div>        
  <div class=""dropMenu"" ><!-- -->
    <div class=""subMenu"" style=""display:inline;"">
      <div class=""subItem""><a href=""/shows-schedule"">Shows Schedule</a></div>
      <div class=""subItem""><a href=""http://www.twitter.com/addic7ed"">Addic7ed@Twitter</a></div>
      <div class=""subItem""><a href=""irc://irc.efnet.net/addic7ed"">Addic7ed on IRC</a></div>
      <div class=""subItem""><a href=""http://chat.mibbit.com/?server=irc.umich.edu&channel=%23addic7ed"">Addic7ed - IRC (WebChat)</a></div>
      <div class=""subItem""><a href=""http://www.facebook.com/pages/Addic7ed-Subtitles/103460216722?ref=nf"">Addic7ed on Facebook</a></div>
      </div>
  </div>
  </div>
  <!-- *********************************End Menu****************************** --></th>
  </tr>
 

</table>
<script type=""text/javascript"" src=""/xpmenuv21.js""></script>
</div>

<center><table border=""0"" width=""90%"">
<tr>
<td class=""NewsTitle""><img width=""20"" height=""20"" src=""https://www.addic7ed.com/images/television.png"" alt=""TV"" /><img src=""https://www.addic7ed.com/images/invisible.gif"" alt="" "" />Addic7ed</td>
<td class=""NewsTitle""><img width=""20"" height=""20"" src=""https://www.addic7ed.com/images/television.png"" alt=""TV"" /><img src=""https://www.addic7ed.com/images/invisible.gif"" alt="" "" />Popular Shows</td>
<td class=""NewsTitle""><img width=""20"" height=""20"" src=""https://www.addic7ed.com/images/television.png"" alt=""TV"" /><img src=""https://www.addic7ed.com/images/invisible.gif"" alt="" "" />Useful</td>
<td class=""NewsTitle""><img width=""20"" height=""20"" src=""https://www.addic7ed.com/images/television.png"" alt=""TV"" /><img src=""https://www.addic7ed.com/images/invisible.gif"" alt="" "" />Forums</td>
</tr>
<tr>
<td><div id=""footermenu""><a href=""/shows.php"">Browse By Shows</a></div></td>
<td><div id=""footermenu""><a href=""https://www.addic7ed.com/show/8439"">Sex/Life</a></div></td>
<td><div id=""footermenu""><a href=""/shows-schedule"">TV Shows Schedule</a></div></td>
<td><div id=""footermenu""><a href=""http://www.sub-talk.net/topic/1031-changelog/"">Site Changelog</a></div></td>
</tr>
<tr>
<td><div id=""footermenu""><a href=""/movie-subtitles"">Browse By Movies</a></div></td>
<td><div id=""footermenu""><a href=""https://www.addic7ed.com/show/8435"">Loki</a></div></td>
<td><div id=""footermenu""><a href=""http://www.sub-talk.net/topic/2784-frequently-asked-questions/"">Frequently Asked Questions</a></div></td>
<td><div id=""footermenu"">Support Us</div></td>
</tr>
<tr>
<td><div id=""footermenu""><a href=""/top-uploaders"">Top Uploaders</a></div></td>
<td><div id=""footermenu""><a href=""https://www.addic7ed.com/show/8470"">Resident Evil</a></div></td>
<td><div id=""footermenu"">RSS Feeds</div></td>
<td><div id=""footermenu"">Premium Accounts</div></td>
</tr>
<tr>
<td><div id=""footermenu""><a href=""/log.php?mode=downloaded"">Top Downloads</a></div></td>
<td><div id=""footermenu""><a href=""https://www.addic7ed.com/show/8428"">Sweet Tooth</a></div></td>
<td class=""NewsTitle""><img width=""20"" height=""20"" src=""https://www.addic7ed.com/images/television.png"" alt=""TV"" /><img src=""https://www.addic7ed.com/images/invisible.gif"" alt="" ""/>Tutorials</td>
<td><div id=""footermenu""><a href=""http://sub-talk.net/thread-6-1-1.html"">Video Formats</a></div></td>
</tr>
<tr>
<td><div id=""footermenu""><a href=""/log.php?mode=news"">All News</a></div></td>
<td><div id=""footermenu""><a href=""https://www.addic7ed.com/show/121"">Gossip Girl</a></div></td>
<td><div id=""footermenu""><a href=""http://www.sub-talk.net/topic/338-guide-to-syncing-with-subtitleedit/page__p__1485__hl__%2B+%2Bsync__fromsearch__1#entry1485"">How to Synchronize Subtitles</a></div></td>
<td><div id=""footermenu"">Frequently Asked Questions</div></td>
</tr> 
<tr>
<td><div id=""footermenu""><a href=""http://www.sub-talk.net"">Sub-Talk Forums</a></div></td>
<td><div id=""footermenu""><a href=""/show/1277"">Shameless (US)</a></div></td>
<td><div id=""footermenu"">What Are Subtitles</div></td>
<td><div id=""footermenu""><a href=""http://sub-talk.net/index.php?gid=7"">TV Shows Talk</a></div></td>
</tr>
<tr>
<td><div id=""footermenu""><a href=""/latest_comments.php"">Latest Comments</a></div></td>
<td><div id=""footermenu""><a href=""/show/126"">The Big Bang Theory</a></div></td>
<td><div id=""footermenu"">New Translation Tutorial</div></td>
<td><div id=""footermenu""><a href=""http://sub-talk.net/index.php?gid=22"">Movies Talk</a></div></td>
</tr>
<tr>
<td><div id=""footermenu""><a href=""https://www.fitint.ro/c/colanti/"" title=""Colanti modelatori"" target=""_blank"" alt=""Colanti modelatori"">Colanti modelatori</a></div></td>
<td><div id=""footermenu""><a href=""/show/130"">Family Guy</a></div></td>
<td><div id=""footermenu"">Upload a New Subtitle Tutorial</div></td>
<td class=""NewsTitle""><img width=""20"" height=""20"" src=""https://www.addic7ed.com/images/television.png"" alt=""TV"" /><img src=""https://www.addic7ed.com/images/invisible.gif"" alt="" "" />Stats</td>
</tr>
<tr>
<td><div id=""footermenu""></div></td>
<td><div id=""footermenu""><a href=""/show/1799"">American Horror Story</a></div></td>
<td><div id=""footermenu""><a href=""http://sub-talk.net/viewthread.php?tid=294"">How to have an Avatar</a></div></td>
<td align=""left"">.
				</td>
</tr>
<tr>
<td><div id=""footermenu""><a href=""/contact.php"">Contact</a></div></td>
<td><div id=""footermenu""><a href=""/show/15"">House</a></div></td>
<td><div id=""footermenu""><a href=""http://www.vreaubagaj.ro/"" alt=""Trolere"" title=""Trolere"">Trolere</a></div></td>
<td>
</td>
</tr>
</table></center>
</center>

<script type=""text/javascript"">
var gaJsHost = ((""https:"" == document.location.protocol) ? ""https://ssl."" : ""http://www."");
document.write(unescape(""%3Cscript src='"" + gaJsHost + ""google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E""));
</script>
<script type=""text/javascript"">
try {
var pageTracker = _gat._getTracker(""UA-10775680-1"");
pageTracker._trackPageview();
} catch(err) {}</script>



                                                                                      
</body>
</html>
";

        #endregion

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));
        var downloadUsage = await _parser.GetDownloadUsageAsync(stream, default);
        downloadUsage.Should().NotBeNull();

        downloadUsage.Used.Should().Be(12);
        downloadUsage.TotalAvailable.Should().Be(40);
        downloadUsage.Remaining.Should().Be(28);
    }
}