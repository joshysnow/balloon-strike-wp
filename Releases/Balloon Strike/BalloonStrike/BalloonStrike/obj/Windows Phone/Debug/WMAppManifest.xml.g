﻿<?xml version="1.0" encoding="utf-8"?>
<Deployment xmlns="http://schemas.microsoft.com/windowsphone/2009/deployment" AppPlatformVersion="7.1">
  <App xmlns="" ProductID="{edc8e2ef-9983-45e5-a116-b20dccfa5adc}" Title="Balloon Strike" RuntimeType="XNA" Version="1.0.0.0" Genre="Apps.Normal" Author="" Description="" Publisher="">
    <IconPath IsRelative="true" IsResource="false">PhoneGameThumb.png</IconPath>
    <Capabilities>
      <Capability Name="ID_CAP_NETWORKING" />
      <Capability Name="ID_CAP_LOCATION" />
      <Capability Name="ID_CAP_SENSORS" />
      <Capability Name="ID_CAP_MICROPHONE" />
      <Capability Name="ID_CAP_MEDIALIB" />
      <Capability Name="ID_CAP_GAMERSERVICES" />
      <Capability Name="ID_CAP_PHONEDIALER" />
      <Capability Name="ID_CAP_PUSH_NOTIFICATION" />
      <Capability Name="ID_CAP_WEBBROWSERCOMPONENT" />
      <Capability Name="ID_CAP_IDENTITY_USER" />
      <Capability Name="ID_CAP_IDENTITY_DEVICE" />
      <!-- Windows Phone OS 7.1 Capabilities -->
      <Capability Name="ID_CAP_ISV_CAMERA" />
      <Capability Name="ID_CAP_CONTACTS" />
      <Capability Name="ID_CAP_APPOINTMENTS" />
    </Capabilities>
    <Tasks>
      <DefaultTask Name="_default" />
    </Tasks>
    <Tokens>
      <PrimaryToken TokenID="BalloonStrikeToken" TaskName="_default">
        <TemplateType5>
          <BackgroundImageURI IsRelative="true" IsResource="false">Background.png</BackgroundImageURI>
          <Count>0</Count>
          <Title>BalloonStrike</Title>
        </TemplateType5>
      </PrimaryToken>
    </Tokens>
  </App>
</Deployment>