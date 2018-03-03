# SpotLight.RayCast
Raycast and detects nearest objects within a spotlight cone

![alt text](https://pbs.twimg.com/media/DXYsWNiV4AEzwA2.jpg:large)

 - Detect Layer: layers it'll grab the closest to.
 - Obsticle Mask Layer: detected go will be ignored if these layers are in front.
 - Detect Facing:
   - Transform Facing: Origin starts at this transform and using it's forward vector.
   - Cam Lock Transform Facing: Origin starts at Main Camera and using it's forward vector. However the yaw will be locked to avoid looking up or down.
   - Cam Forward: Origin starts at Main Camera and using it's forward vector.
 - Detect Distance: Distance to detect initialy.
 - Lost Distance: Target fully forget past this point if target travels too far away.
 - Detect Angle: Cone spread to detect.
 - Sight Height: Offset height of origin and target.

 - Update Ticker: Per second refresh. Optimization to avoid raycasting every frame.

//results//
 - In Sight Target: (GameObject) Target is in sight, but dissapears with obsticles or not within the cone.
 - Detect Target: (GameObject) Recent target will stay in memory, regardless of obsticles or out of cone. Will dissapear if target is out of 'Lost Distance' value.
 - Dist Target: (float)Distance value of detected target.


Free to use. Please mention my name "Eugene Chu" twitter: @LenZ_Chu if you can ;3 https://twitter.com/LenZ_Chu
