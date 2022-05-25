using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*
 * This page allows a person to find which electorates they live in,
 * and hence which MPs represent them.
 *
 * This is used in two possible places:
 * (1) if the person clicks on 'My MP' when setting question metadata,
 * we need to know who their MPs are. After this page,
 * there will be a list of MPs loaded for them to choose from.
 * This is indicated by setting LaunchMPsSelectionPageNext to true.
 * 
 * (2) if the person tries to vote or post a question.
 * In this case, they have generated a name via RegisterPage1
 * and can skip this step (so showSkip should be set to true).
 * And we don't follow with a list of MPs for them to chose from.
 */
namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage2 : ContentPage
    {
        // private ParliamentData.Chamber stateLCChamber=ParliamentData.Chamber.Vic_Legislative_Council;
        // private ParliamentData.Chamber stateLAChamber=ParliamentData.Chamber.Vic_Legislative_Assembly;
        // alreadySelectedMPs are passed in if a Selection page is to be launched next.
        // If they're null/absent, no selection page is launched.
        public RegisterPage2()
        {
            InitializeComponent();
//            testWebView.Source = "https://www.abc.net.au/res/sites/news-projects/interactive-electorateboundaries-2/5.0.0/?kml=/dat/news/elections/federal/2022/guide/kml/Sydney.kml";
//            var htmlSource = new HtmlWebViewSource();
//            htmlSource.Html = @"<div class=""map maplibregl-map mapboxgl - map"">
//<div class=""maplibregl - canvas - container mapboxgl - canvas - container maplibregl - interactive mapboxgl - interactive maplibregl - touch - drag - pan mapboxgl - touch - drag - pan maplibregl - touch - zoom - rotate mapboxgl - touch - zoom - rotate"">
//<canvas class=""maplibregl - canvas mapboxgl - canvas"" tabindex=""0"" aria-label=""Map"" role=""region"" width=""150"" height=""492"" style=""width: 150px; height: 492px; ""></canvas></div><div class=""maplibregl - control - container mapboxgl - control - container"">
//<div class=""maplibregl - ctrl - top - left mapboxgl - ctrl - top - left""></div><div class=""maplibregl - ctrl - top - right mapboxgl - ctrl - top - right""></div><div class=""maplibregl - ctrl - bottom - left mapboxgl - ctrl - bottom - left"">
//<div class=""maplibregl - ctrl maplibregl - ctrl - group mapboxgl - ctrl mapboxgl - ctrl - group""><button class=""maplibregl - ctrl - zoom -in mapboxgl - ctrl - zoom -in"" type=""button"" title=""Zoom in"" aria-label=""Zoom in"" aria-disabled=""false"">
//<span class=""maplibregl - ctrl - icon mapboxgl - ctrl - icon"" aria-hidden=""true""></span></button><button class=""maplibregl - ctrl - zoom -out mapboxgl - ctrl - zoom -out"" type=""button"" title=""Zoom out"" aria-label=""Zoom out"" aria-disabled=""false"">
//<span class=""maplibregl - ctrl - icon mapboxgl - ctrl - icon"" aria-hidden=""true""></span></button><button class=""maplibregl - ctrl - compass mapboxgl - ctrl - compass"" type=""button"" title=""Reset bearing to north"" aria-label=""Reset bearing to north"">
//<span class=""maplibregl - ctrl - icon mapboxgl - ctrl - icon"" aria-hidden=""true"" style=""transform: rotate(0deg); ""></span></button></div><div class=""maplibregl - ctrl mapboxgl - ctrl"" style=""display: none; "">
//<a class=""maplibregl - ctrl - logo mapboxgl - ctrl - logo maplibregl - compact mapboxgl - compact"" target=""_blank"" rel=""noopener nofollow"" href=""https://maplibre.org/"" aria-label=""Mapbox logo"">
//</a></div></div><div class=""maplibregl-ctrl-bottom-right mapboxgl-ctrl-bottom-right""><div class=""maplibregl-ctrl maplibregl-ctrl-attrib mapboxgl-ctrl mapboxgl-ctrl-attrib maplibregl-compact mapboxgl-compact"">
//<button class=""maplibregl-ctrl-attrib-button mapboxgl-ctrl-attrib-button"" type=""button"" title=""Toggle attribution"" aria-label=""Toggle attribution""></button>
//<div class=""maplibregl-ctrl-attrib-inner mapboxgl-ctrl-attrib-inner"" role=""list"">© ABC | <a href=""https://www.openmaptiles.org/"" target=""_blank"">© OpenMapTiles</a> 
//<a href=""https://www.openstreetmap.org/copyright"" target=""_blank"">© OpenStreetMap contributors</a></div></div></div></div></div>";
//            testWebView.Source = htmlSource.Html;
        }
    }
}