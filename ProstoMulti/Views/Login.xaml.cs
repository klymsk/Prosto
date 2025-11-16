using System.Net;
using System.Text;

namespace ProstoMulti.Views;

public partial class Login : ContentPage
{
    private HttpListener listener;

    public Login()
    {
        InitializeComponent();
        StartLocalServer();
    }

    private async void StartLocalServer()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5005/callback/");
        listener.Start();

        _ = Task.Run(async () =>
        {
            while (true)
            {
                var context = await listener.GetContextAsync();
                var req = context.Request;
                var res = context.Response;

                var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

                string fullName = query["fullName"];
                string email = query["email"];

                byte[] buffer = Encoding.UTF8.GetBytes("<h1>Успішно! Можете закрити це вікно.</h1>");
                res.OutputStream.Write(buffer);
                res.Close();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    FullNameLabel.Text = $"Name: {fullName}";
                    EmailLabel.Text = $"Email: {email}";
                });

                break; 
            }

            listener.Stop();
        });
    }

    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        string url = "http://localhost:5297/api/v1/userprofile/LoginWithGoogle";
        await Launcher.OpenAsync(url);
    }
}