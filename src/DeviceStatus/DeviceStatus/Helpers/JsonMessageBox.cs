using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DeviceStatus.Helpers
{
    public partial class JsonMessageBox : Form
    {
        public static JsonMessageBox messageBox = null;

        public JsonMessageBox()
        {
            InitializeComponent();
        }

        public void SetLabel(string label)
        {
            this.label1.Text = label;
            this.Text = label;
        }
        public void SetContents(string contents)
        {
            richTextBox1.Text = contents;
        }
        public static void Highlight(RichTextBox theBox, List<string> highlights)
        {
            PreHighlight(theBox);
            HighLightLine(theBox, "MessageID");
            if (highlights != null)
                foreach (string highlight in highlights)
                    HighLightLine(theBox, highlight);
        }

        /// <summary>
        /// Tag Highlighting is broken unless we set all font to regular before starting.
        /// Reason for broken is unknown, and not worth investigation.
        /// </summary>
        /// <param name="theBox"></param>
        public static void PreHighlight(RichTextBox theBox)
        {
            theBox.SelectAll();
            theBox.SelectionFont = new Font(theBox.Font, FontStyle.Regular);
            theBox.Select(0, 0);
        }

        public static void HighLightLine(RichTextBox theBox, string theTag)
        {
            int end = 0;
            int begin = 0;

            while (end >= 0)
            {
                string tagString = $"\"{theTag}\"";
                int beginTag = theBox.Text.IndexOf(tagString, begin);
                if (beginTag > 0)
                {
                    int prevNewLine = beginTag;
                    int postNewLine = theBox.Text.IndexOf("\n", beginTag); //RTB uses \n not \r\n
                    if (prevNewLine > 0 && postNewLine > 0)
                    {
                        char charBeforeNewLine = theBox.Text[postNewLine - 1];
                        begin = prevNewLine;
                        end = postNewLine;
                        if (charBeforeNewLine == ',')
                            end--;
                    }
                }
                if (begin < end)
                {
                    theBox.Select(begin, end - begin);
                    theBox.SelectionFont = new Font(theBox.Font, FontStyle.Bold);
                    theBox.Select(0, 0);
                    begin = end;
                }
                else
                    end = -1;
            }
        }

        public static string[] GetLine(RichTextBox theBox, string theTag)
        {
            List<string> output = new List<string>();
            int end = 0;
            int begin = 0;

            while (end >= 0)
            {
                string tagString = $"\"{theTag}\"";
                int beginTag = theBox.Text.IndexOf(tagString, begin);
                if (beginTag > 0)
                {
                    int prevNewLine = beginTag;
                    int postNewLine = theBox.Text.IndexOf("\n", beginTag); //RTB uses \n not \r\n
                    if (prevNewLine > 0 && postNewLine > 0)
                    {
                        char charBeforeNewLine = theBox.Text[postNewLine - 1];
                        begin = prevNewLine;
                        end = postNewLine;
                        if (charBeforeNewLine == ',')
                            end--;
                    }
                }
                if (begin < end)
                {
                    output.Add(theBox.Text.Substring(begin, end - begin));
                    begin = end;
                }
                else
                    end = -1;
            }
            return output.ToArray();
        }
        public static void Show(string fname, string json, List<string> highlights = null)
        {
            TCLinkResponse response = Helper.DeserializeLinkResponse(json);

            json = response == null ? json : Helper.Stringify(response);
            if (json.IndexOf("{") == 0)
            {
                json = RemoveCardWorkflowControls(json);
            }
            messageBox = new JsonMessageBox();
            messageBox.SetLabel(fname);
            messageBox.SetContents(json);
            Highlight(messageBox.richTextBox1, highlights);
            messageBox.Show();
        }

        public static void CloseForm()
        {
            if (messageBox != null)
            {
                messageBox.Close();
                messageBox = null;
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static void RemoveFields(JToken token, string[] fields)
        {
            JContainer container = token as JContainer;
            if (container == null)
            {
                return;
            }

            List<JToken> removeList = new List<JToken>();
            foreach (JToken el in container.Children())
            {
                JProperty p = el as JProperty;
                if (p != null && fields.Contains(p.Name))
                {
                    removeList.Add(el);
                }
                RemoveFields(el, fields);
            }

            foreach (JToken el in removeList)
            {
                el.Remove();
            }
        }

        private static string RemoveCardWorkflowControls(string payloadString)
        {
            //TODO: please check and make sure we do need this functionality. TestHarness should not manipulate the data from IPA5.
            try
            {
                var jsonObject = JObject.Parse(payloadString);

                JArray jResponsesListArray = jsonObject["Responses"] as JArray;
                if (jResponsesListArray != null)
                {
                    JObject jDALResponse = jResponsesListArray.First["DALResponse"] as JObject;
                    if (jDALResponse != null)
                    {
                        JArray jDevices = jDALResponse["Devices"] as JArray;
                        if (jDevices != null)
                        {
                            RemoveFields(jDevices, new string[] { "CardWorkflowControls" });
                        }
                    }
                }

                return jsonObject.ToString();
            }
            catch
            {
                return payloadString;
            }
        }
    }
}
