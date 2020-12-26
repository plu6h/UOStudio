﻿using System;
using System.Linq;
using ImGuiNET;
using UOStudio.Client.Core;
using Num = System.Numerics;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public sealed class MapConnectToServerWindow : Window
    {
        private readonly ProfileService _profileService;

        public event EventHandler<ConnectEventArgs> ConnectClicked;

        public event EventHandler DisconnectClicked;

        public event Action EditProfilesClicked;

        private string _serverName;
        private string _serverPort;
        private string _userName;
        private string _password;
        private int _selectedProfileIndex;
        private Profile _selectedProfile;

        public MapConnectToServerWindow(ProfileService profileService)
            : base("Login")
        {
            _profileService = profileService;
            _serverName = string.Empty;
            _serverPort = string.Empty;
            _userName = string.Empty;
            _password = string.Empty;
        }

        public Profile SelectedProfile
        {
            get => _selectedProfile;
            set => _selectedProfile = value;
        }

        public string ServerName
        {
            get => _serverName;
            set => _serverName = value;
        }

        public int ServerPort
        {
            get => int.TryParse(_serverPort, out var port) ? port : 0;
            set => _serverPort = value.ToString();
        }

        public string UserName
        {
            get => _userName;
            set => _userName = value;
        }

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        protected override ImGuiWindowFlags GetWindowFlags() => ImGuiWindowFlags.NoDocking;

        protected override void DrawInternal()
        {
            var backBufferSize = ImGui.GetWindowViewport().Size;
            var windowSize = ImGui.GetWindowSize();
            ImGui.SetWindowPos(
                new Num.Vector2(backBufferSize.X / 2.0f - windowSize.X / 2.0f, backBufferSize.Y / 2.0f - windowSize.Y / 2.0f)
            );

            var profileNames = _profileService.GetAll(p => p.Name).ToArray();
            if (ImGui.Combo("Profiles", ref _selectedProfileIndex, profileNames, Math.Min(5, profileNames.Length)))
            {
                _selectedProfile = _profileService.GetByIndex(_selectedProfileIndex);
                _serverName = _selectedProfile?.ServerName ?? string.Empty;
                _serverPort = _selectedProfile?.ServerPort.ToString() ?? string.Empty;
                _userName = _selectedProfile?.AccountName ?? string.Empty;
                _password = _selectedProfile?.AccountPassword ?? string.Empty;
            }

            if (ImGui.Button("Edit Profiles"))
            {
                var editProfilesClicked = EditProfilesClicked;
                editProfilesClicked?.Invoke();
            }

            if (ImGui.InputText("Server", ref _serverName, 64))
            {
            }

            if (ImGui.InputText("Port", ref _serverPort, 5))
            {
            }

            if (ImGui.InputText("User Name", ref _userName, 64))
            {
            }

            if (ImGui.InputText("Password", ref _password, 64, ImGuiInputTextFlags.Password))
            {
            }

            if (ImGui.Button("Connect"))
            {
                var serverPortAsInt = int.TryParse(_serverPort, out var port) ? port : 0;

                var connectClicked = ConnectClicked;
                connectClicked?.Invoke(this, new ConnectEventArgs(_serverName, serverPortAsInt, _userName, _password));
            }

            ImGui.SameLine();
            if (ImGui.Button("Disconnect"))
            {
                var disconnectClicked = DisconnectClicked;
                disconnectClicked?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}