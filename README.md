# WorldBox++

[![CircleCI](https://dl.circleci.com/status-badge/img/gh/apolo49/worldboxpp/tree/master.svg?style=svg)](https://dl.circleci.com/status-badge/redirect/gh/apolo49/worldboxpp/tree/master)
[![pre-commit](https://img.shields.io/badge/pre--commit-enabled-brightgreen?logo=pre-commit&logoColor=white)](https://github.com/pre-commit/pre-commit)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=worldboxpp&metric=coverage)](https://sonarcloud.io/summary/new_code?id=worldboxpp)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=worldboxpp&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=worldboxpp)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=worldboxpp&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=worldboxpp)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=worldboxpp&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=worldboxpp)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=worldboxpp&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=worldboxpp)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=worldboxpp&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=worldboxpp)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=worldboxpp&metric=bugs)](https://sonarcloud.io/summary/new_code?id=worldboxpp)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=worldboxpp&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=worldboxpp)

WorldBox++ is a mod for WorldBox with the goal of optimising WorldBox
with larger maps and adding new features to enhance the worldbox experience.

This mod makes heavy use of reflection and 0Harmony. Therefore due to the aformentioned
practices may or may not be compatible with other WorldBox mods.

## Branch Status

Master:
[![CircleCI](https://dl.circleci.com/status-badge/img/gh/apolo49/worldboxpp/tree/master.svg?style=svg)](https://dl.circleci.com/status-badge/redirect/gh/apolo49/worldboxpp/tree/master)

feature/cultureDiverge:
[![CircleCI](https://dl.circleci.com/status-badge/img/gh/apolo49/worldboxpp/tree/feature%2FcultureDiverge.svg?style=svg)](https://dl.circleci.com/status-badge/redirect/gh/apolo49/worldboxpp/tree/feature%2FcultureDiverge)

## Technology

WorldBox++ makes use of static analysis to enforce code quality and linting to
ensure proper and correct practices are used.

The repository makes use of precommit hooks to stop anything from being pushed
to master and to be sure the linter runs beforehand to prevent bad code pollution.

WorldBox++ undergoes automated testing and building using a CI pipeline to
produce artifacts on every push.

### Pre-commit

Pre-commit runs hooks on every commit to automatically point out issues in code
and resolve them if possible. This reduces the amount of effort that needs to
go into a code review (not eliminate).

This allows you to have linters run automatically and static analysis take
place in the CI pipeline and on the commit meaning less buggy and more
efficient, readable and understandable code.

#### Installation

##### Windows

pip or conda needs to be installed for windows installation.

```bash
pip install pre-commit
```

```bash
conda install -c conda-forge pre-commit
```

##### MacOS / Linux

Homebrew is used to install pre-commit on these devices where possible

```bash
brew install pre-commit
```

### Optimisation

#### Quadtrees

This repo makes use of a quadtree library made by splitice found
[here](https://github.com/splitice/QuadTrees).

Quadtrees will be used to minimise and optimise computation on individual tiles
by grouping tiles together into one to reduce rendering costs. This is used in
lossless AND lossy picture compression.

## Features

### Cultural Divergence

Cultural divergence is the first feature developed for the mod and features a
new "world law" and custom values for how long it takes for a culture to
potentially start to diverge from another.

Upon a divergence taking place a single city will develop a new culture and all
zones, and citizens of the city (of the parent culture), will become of the new
culture.

This allows for a more dynamic game, although if it is volatile enough there is
a chance that the every village will be a different culture.

### Contributing

Contributions of all kinds are welcomed and are encouraged. Though to begin
development of the mod a few tools must be installed beforehand.

Required:

- Worldbox
- Docker
- NCMS
- DotNET 6.0.100-preview.1.21103.13
- Python 3.10
- Git
- Visual Studio 2022

Recommended (Windows Machines):

- Git for Windows
- Git bash
- Windows Terminal
- Docker Desktop

#### Setup

Clone the repo to your local machine

```bash
$git clone git@github.com:apolo49/worldboxpp.git --recurse-submodules
```

Set up the virtual environment

```bash
py -m venv venv && . ./venv/Scripts/activate && py -m pip install pip --upgrade
```

Set up pre-commit hooks

```bash
py -m pip install pre-commit && pre-commit install
```

Whenever going to code in this repo, be sure to activate your virtual
environment before starting or committing.

## License

Copyright 2022 Joe Targett

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

[http://www.apache.org/licenses/LICENSE-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
