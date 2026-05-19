
SPACE SIM ECS/BURST

Features:
- ECS + Burst
- Custom gravity
- Elliptical orbits
- Saturn-style asteroid rings
- Simplified black hole relativity
- 3D simulation constrained to Y=0

REQUIREMENTS:
- Unity 6 or Unity 2022.3+
- Entities package
- Burst package
- Mathematics package

Suggested setup:
- Create sphere prefabs
- Add CelestialBodyAuthoring
- Create a massive central star
- Add AsteroidRingSpawner
- Mark black holes with isBlackHole=true

NOTES:
- Uses custom physics
- Does NOT use Rigidbody
- Optimized for thousands of bodies
