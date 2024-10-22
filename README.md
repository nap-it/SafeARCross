# SafeARCross

SafeARCross is an AR application designed for the Microsoft HoloLens 2 aimed to improve awareness in pedestrian crossing scenarios using two systems, (1) a collision warning system, and (2) virtual traffic lights. Our analysis of this system showed that there was statistical evidence suggesting significant improvements in perceived workload, perceived safety, and a reduction in the number of head movements for these systems when compared to scenarios without AR assistance, concluding that the users trusted
the AR systems. Furthermore, participants rated the systems as excellent in terms of usability.

The Collision Warning System AR interface is designed to alert the user through a multi-modal warning system, that is activated when it receives a new DENM via MQTT, immediately alerting the user with: (1) a visual notification within the user’s center of view prompting to halt; (2) a 3D arrow pointing to the direction of the hazard; (3) and an 800 Hz tone lasting one second. As DENMs are continuously being transmitted during hazardous events, the visual alert remains active, only disappearing when a new message has not been received for more than one second.

The Virtual Traffic Lights displayed inform Vulnerable Road users (VRUs) of vehicle intentions, enhancing crossing safety. The interface signals when it is unsafe to cross by using visual cues and a bell-like sound to capture attention when the traffic light changes. The system relies on exchanging data through MQTT such as VRU Awareness Messages (VAMs), Signal Phase and Timing Messages (SPATEMs), and MAP Extended Messages (MAPEMs), which describe intersection layouts and traffic light phases. By analyzing these messages, the system determines the VRU's lane and displays the appropriate traffic signal accordingly.
<br>

## Getting started
The project contains the binary .sln file ([SafeARCross.sln](https://github.com/nap-it/SafeARCross/blob/main/SafeARCross.sln)) ready to be deployed on the Microsoft HoloLens 2 using Visual Studio 2019. Follow the Microsoft [guidelines](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/advanced-concepts/using-visual-studio?tabs=hl2) to deploy and debug using Visual Studio.
<br>

## Installation/Usage

To be able to compile this code first you must follow Microsoft's MRTK Unity [setup](https://learn.microsoft.com/en-us/training/modules/learn-mrtk-tutorials/1-3-exercise-configure-unity-for-windows-mixed-reality), clone the repository code and then follow the [build and deploy instructions](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/build-and-deploy-to-hololens).
<br>

## Dependencies
- [MRTK3](https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk3-overview/): Framework used on the Microsoft HoloLens 2.
- [M2MqttUnity](https://github.com/CE-SDV/M2MqttUnity): This package is used to interact with MQTT messages (either publishing or subscribing).
- [UnityFigmaBridge](https://github.com/simonoliver/UnityFigmaBridge): This plugin was used to import the visual assets created for the Collision Warning System in Figma to the Unity Scene.
- [glTFast](https://github.com/atteneder/glTFast): This plugin was used to import the 3D arrow constructed in Blender (it .gltf format) to the Unity Scene.

## Demonstration Video
Check out the [demonstration video](https://youtu.be/PiCTL-jrqdQ) for this system and subsequent work done exploring its effects on VRU safety. 

## Authors and acknowledgement
This project is a collaborative effort with the following authors who have contributed to the development and success of this work: André Clérigo, Maximilian Schrapel, Pedro Teixeira, Pedro Rito, Susana Sargento, Alexey Vinel.

For any use of this code, data, or design, please refer to the [CITATION.cff](https://github.com/nap-it/SafeARCross/blob/main/CITATION.cff) file in the repository to properly acknowledge and cite the contributions of these authors.
<br>

## License
This project is licensed under the GNU General Public License v3.0. The full license can be viewed in the [LICENSE](https://github.com/nap-it/SafeARCross/blob/main/LICENSE) file included in this repository.

