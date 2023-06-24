using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

public class LiveSplitXMLReader
{
    private const string GAME_NAME = "GameName";
    private const string CATEGORY_NAME = "CategoryName";
    private const string OFFSET = "Offset";
    private const string ATTEMPTS_COUNT = "AttemptCount";
    private const string SEGMENTS = "Segments";
    private const string SEGMENT_NAME = "Name";
    private const string SPLIT_TIMES = "SplitTimes";
    private const string BEST_SEGMENT = "BestSegmentTime";

    private readonly RunSplits split;
    private readonly XmlDocument xmlDocument;

    public LiveSplitXMLReader()
    {
        split = new RunSplits();
        xmlDocument = new XmlDocument();
    }

    /// <summary>
    /// Read split information from the file and returns it.
    /// </summary>
    /// <param name="file">The filename of the LiveSplit XML file.</param>
    /// <returns>The split with all its information.</returns>
    public RunSplits ReadSplit(string file)
    {
        xmlDocument.Load(file);
        StringBuilder stringBuilder = new StringBuilder();
        string runTitle = "";
        string attemptsCountString = "";
        string startDelay = "";
        int attemptsCount = 0;
        XmlNodeList rootNode;
        List<SplitSegment> segments = new List<SplitSegment>();

        rootNode = xmlDocument.DocumentElement.SelectNodes("/Run");
        foreach (XmlNode runNode in rootNode)
        {
            foreach (XmlNode infoNode in runNode.ChildNodes)
            {
                if (infoNode.Name == GAME_NAME)
                {
                    stringBuilder.Append(infoNode.InnerText + " ");
                }
                else if (infoNode.Name == CATEGORY_NAME)
                {
                    stringBuilder.Append(infoNode.InnerText);
                }
                else if (infoNode.Name == OFFSET)
                {
                    startDelay = infoNode.InnerText;
                }
                else if (infoNode.Name == ATTEMPTS_COUNT)
                {
                    attemptsCountString = infoNode.InnerText;
                }
                else if (infoNode.Name == SEGMENTS)
                {
                    PopulateSegments(segments, infoNode);
                }
            }
        }

        runTitle = stringBuilder.ToString();
        split.RunTitle = runTitle;
        split.StartDelay = SetRunDelay(startDelay);
        int.TryParse(attemptsCountString, out attemptsCount);
        split.AttemptsCount = attemptsCount;
        split.segments = segments;
        return split;
    }

    /// <summary>
    /// In livesplit you can have a delay or start the run later. In wsplit you can only delay the start of a run.
    /// So in livesplit if there is a delay, there will be a "-" in front of the time. If we see this "-" we read after the time after it
    /// to set the time delay in the Split. If there is not "-" it means that the run start later and we do nothing with it.
    /// </summary>
    /// <param name="delayString">The initial delay string from the file.</param>
    /// <returns>The delay into the form of a int.</returns>
    private int SetRunDelay(string delayString)
    {
        string delayStringModified = "";
        int delay = 0;

        if (delayString.IndexOf('-') != -1)
        {
            delayStringModified = delayString.Remove(0, 1);
            delay = ParseDelayString(delayStringModified);
        }

        return delay;
    }

    /// <summary>
    /// Take the delay string from the livesplit file and convert it into int.
    /// </summary>
    /// <param name="delayString">The delay into the form of a String.</param>
    /// <returns>The delay into the form of a int.</returns>
    private int ParseDelayString(string delayString)
    {
        int delay = 0;
        int delayTimeSection = 0;
        string[] timeSection = delayString.Split(':');
        string[] secondsAndMilliseconds = timeSection[timeSection.Length - 1].Split('.');
        //Millseconds
        if (secondsAndMilliseconds.Length == 2)
        {
            if (int.TryParse(secondsAndMilliseconds[1].Substring(0, 2), out delayTimeSection))
            {
                delay += (delayTimeSection * 10);
            }
        }
        //Seconds
        if (int.TryParse(secondsAndMilliseconds[0], out delayTimeSection))
        {
            delay += (delayTimeSection * 1000);
        }
        //Minutes
        if (int.TryParse(timeSection[1], out delayTimeSection))
        {
            delay += (delayTimeSection * 60 * 1000);
        }
        //Hours
        if (int.TryParse(timeSection[0], out delayTimeSection))
        {
            delay += (delayTimeSection * 3600 * 1000);
        }
        return delay;
    }

    /// <summary>
    /// Read the segments from the file and populate the segment list.
    /// </summary>
    /// <param name="segments">The array list of segments.</param>
    /// <param name="segmentsNode">The node containing the segments in the xml file.</param>
    private void PopulateSegments(List<SplitSegment> segments, XmlNode segmentsNode)
    {
        SplitSegment newSegment;
        string segmentName = "";
        double segmentBestTime = 0.0;
        double segmentBestSegment = 0.0;
        XmlNode nodeSegmentTime;
        foreach (XmlNode segmentNode in segmentsNode.ChildNodes)
        {
            foreach (XmlNode segmentInfoNode in segmentNode.ChildNodes)
            {
                if (segmentInfoNode.Name == SEGMENT_NAME)
                {
                    segmentName = segmentInfoNode.InnerText;
                }
                else if (segmentInfoNode.Name == SPLIT_TIMES)
                {
                    nodeSegmentTime = segmentInfoNode.FirstChild.FirstChild;
                    segmentBestTime = TimeParse(nodeSegmentTime.InnerText);
                }
                else if (segmentInfoNode.Name == BEST_SEGMENT)
                {
                    nodeSegmentTime = segmentInfoNode.FirstChild;
                    segmentBestSegment = TimeParse(nodeSegmentTime.InnerText);
                }
            }
            newSegment = new SplitSegment(segmentName, 0.0, segmentBestTime, segmentBestSegment);
            segments.Add(newSegment);
        }
    }

    private static double TimeParse(string timeString)
    {
        double num = 0.0;
        foreach (string str in timeString.Split(new char[] { ':' }))
        {
            double num2;
            if (double.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture(""), out num2))
                num = (num * 60.0) + num2;
        }
        return num;
    }
}