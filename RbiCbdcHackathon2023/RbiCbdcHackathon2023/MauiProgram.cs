﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using RbiCbdcHackathon2023.Database;
using RbiCbdcHackathon2023.Helper.ValueConverter;
using RbiCbdcHackathon2023.Pages;
using RbiCbdcHackathon2023.Pages.Popups;
using RbiCbdcHackathon2023.Services;
using RbiCbdcHackathon2023.ViewModels;

namespace RbiCbdcHackathon2023
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<RegisterPage>();
            builder.Services.AddSingleton<RegisterViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<LoadMoneyViewModel>();
            builder.Services.AddSingleton<DatabaseContext>();
            builder.Services.AddTransient<SendMoneyPopup>();
            //Application.Current.Resources.Add("EpochToDateConverter", new EpochToDateConverter());
            return builder.Build();
        }
    }
}