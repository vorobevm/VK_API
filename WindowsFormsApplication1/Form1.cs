using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestSharp;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Threading;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        private readonly RestClient _restClient;
        private readonly SqlConnection dbConnect;

        public Form1()
        {
            InitializeComponent();
            _restClient = new RestClient("https://api.vk.com/method/");
            dbConnect = new SqlConnection("data source=.\\mssql14;initial catalog=reps;integrated security=false;user id=vkuser;password=vku$er;connect timeout=60000;encrypt=False;trustservercertificate=False;MultipleActiveResultSets=True;App=EntityFramework");
            dbConnect.Open();
        }

        class vkresponse
        {
            [JsonProperty("response")]
            public string[] resp;
        }

        private string[] GetFriendList(string id)
        {
            Thread.Sleep(350);
            var response = _restClient.Post<object>(
                   new RestRequest("friends.get?user_id=" + id, Method.POST));
            var list = ((vkresponse)JsonConvert.DeserializeObject(response.Content, typeof(vkresponse))).resp

            return ;
        }

        private void PutFriend(string userid1, string userid2)
        {
            SqlCommand sql = new SqlCommand("INSERT INTO reps.dbo.user_links (userid1, userid2) VALUES (@userid1, @userid2)", dbConnect);
            sql.Parameters.AddWithValue("@userid1", userid1);
            sql.Parameters.AddWithValue("@userid2", userid2);
            sql.ExecuteNonQuery();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string userid1 = this.textBox1.Text;
            foreach (var friend in GetFriendList(userid1))
            {
                PutFriend(userid1, friend);
                var friendList = GetFriendList(friend);
                if (friendList.Length > 0)
                {
                    foreach (var fr2 in GetFriendList(friend))
                    {
                        PutFriend(friend, fr2);
                    }
                }
                
            }
            MessageBox.Show("Done", "Crawling finished", MessageBoxButtons.OK);

        }
    }
}
