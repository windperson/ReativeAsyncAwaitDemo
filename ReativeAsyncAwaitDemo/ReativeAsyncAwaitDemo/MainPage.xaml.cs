using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using dotMorten.Xamarin.Forms;
using DuckDuckGo.Net;
using Xamarin.Forms;

namespace ReativeAsyncAwaitDemo
{
    public partial class MainPage : ContentPage
    {
        private readonly Search _search;
        private readonly IDisposable _subscribe;

        public MainPage()
        {
            InitializeComponent();
            _search = new Search
            {
                NoHtml = true,
                NoRedirects = true,
                IsSecure = true,
                SkipDisambiguation = false,
            };
            _subscribe = SetupUIeventStream();
        }

        ~MainPage()
        {
            _subscribe?.Dispose();
        }

        private IDisposable SetupUIeventStream()
        {
            return Observable.FromEventPattern<EventHandler<AutoSuggestBoxTextChangedEventArgs>, AutoSuggestBoxTextChangedEventArgs>(
                      ev => SearchTextBox.TextChanged += ev,
                      ev => SearchTextBox.TextChanged -= ev)
                  .Throttle(TimeSpan.FromMilliseconds(500))
                  .Where(x => x.EventArgs.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                  .Select(x => ((AutoSuggestBox)x.Sender).Text)
                  .Where(txt => !string.IsNullOrWhiteSpace(txt) && txt.Length >= 3)
                  .DistinctUntilChanged()
                  .SelectMany(queryStr => GetSuggestions(queryStr, _search).ToObservable())
                  .Subscribe(value =>
                  {
                      Device.BeginInvokeOnMainThread(() =>
                    {
                         Debug.WriteLine("update suggestion list");
                         SearchTextBox.ItemsSource = value.ToList();
                     });
                  });
        }

        private static async Task<IEnumerable<QueryResult>> GetSuggestions(string searchText, Search search)
        {
            Debug.WriteLine("invoke Web API");
            var searchResult = await search.QueryAsync(searchText, Guid.NewGuid().ToString());
            return searchResult.RelatedTopics.Where(x => !string.IsNullOrEmpty(x.Text));
        }


        //private async void AutoSuggestBox_TextChanged(object sender, AutoSuggestBoxTextChangedEventArgs e)
        //{
        //    var searchBox = (AutoSuggestBox) sender;

        //    if (e.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
        //    {
        //        return;
        //    }
        //    var searchText = searchBox.Text;
        //    if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 3)
        //    {
        //        searchBox.ItemsSource = null;
        //    }
        //    else
        //    {
        //        var suggestions = await GetSuggestions(searchText);
        //        searchBox.ItemsSource = suggestions.ToList();
        //    }
        //}

        //private async Task<IEnumerable<QueryResult>> GetSuggestions(string searchText)
        //{
        //    var searchResult = await _search.QueryAsync(searchText, Guid.NewGuid().ToString());
        //    return searchResult.RelatedTopics.Where(x => !string.IsNullOrEmpty(x.Text));
        //}




        //private void AutoSuggestBox_QuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        //{
        //    //not implement
        //}

        //private void AutoSuggestBox_SuggestionChosen(object sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        //{
        //    // not implment
        //}
    }
}
