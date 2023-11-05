﻿using TI2._1_HealthCareClient.Commands;

namespace TI2._1_HealthCareClient.VRConnection.Commands.Panel.DataAttributes
{
    public class ScenePanelImage : IVRCommand
    {
        public string Id { get { return "scene/panel/image"; } }
        public ScenePanelImageDataAttribute Data { get; set; }
    }
}
