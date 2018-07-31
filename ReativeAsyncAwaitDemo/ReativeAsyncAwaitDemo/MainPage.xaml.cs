using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotMorten.Xamarin.Forms;
using DuckDuckGo.Net;
using Xamarin.Forms;

namespace ReativeAsyncAwaitDemo
{
    public partial class MainPage : ContentPage
    {
        private Search _search;


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
        }

        private async void AutoSuggestBox_TextChanged(object sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            var searchBox = (AutoSuggestBox) sender;

            if (e.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            {
                return;
            }
            var searchText = searchBox.Text;
            if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 3)
            {
                searchBox.ItemsSource = null;
            }
            else
            {
                var suggestions = await GetSuggestions(searchText);
                searchBox.ItemsSource = suggestions.ToList();
            }
        }

        private async Task<IEnumerable<QueryResult>> GetSuggestions(string searchText)
        {
            var searchResult = await _search.QueryAsync(searchText, Guid.NewGuid().ToString());
            return searchResult.RelatedTopics.Where(x => !string.IsNullOrEmpty(x.Text));
        }

        private void AutoSuggestBox_QuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            //not implement
        }

        private void AutoSuggestBox_SuggestionChosen(object sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            // not implment
        }
    }
}
