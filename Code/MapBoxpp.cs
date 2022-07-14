using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Worldboxpp.Culturepp;

namespace Worldboxpp
{
    public class MapBoxpp : MapBox
    {
        new public CultureManagerpp cultures;
        new public static MapBoxpp instance;
        private SignalManager _signalManager;
        private IslandsCalculator islandsCalculator;
        private SimObjectsZones simObjectsZones;
        private CityPlaceFinder cityPlaceFinder;
        private QualityChanger qualityChanger;
        private Transform transformCreatures;
        private Transform transformUnits;
        private Transform transformBuildings;
        private Transform transformShadows;
        private Transform transformTrees;
        private List<WorldTile> tilesDirty;
        private ZoneCamera zone_camera;
        private Heat heat;

        private bool Awake()
        {
            instance = this;
            MapAction.init(this);
            _signalManager = new SignalManager();
            base.joys.gameObject.SetActive(value: false);
            base.magnet = new Magnet(this);
            base.unitGroupManager = new UnitGroupManager(this);
            islandsCalculator = new IslandsCalculator(this);
            simObjectsZones = new SimObjectsZones(this);
            base.worldLog = new WorldLog(this);
            cityPlaceFinder = new CityPlaceFinder(this);
            qualityChanger = GetComponent<QualityChanger>();
            transformCreatures = GameObject.Find("Creatures").transform;
            transformUnits = transformCreatures.Find("Units").transform;
            transformBuildings = GameObject.Find("Buildings").transform;
            transformShadows = GameObject.Find("Shadows").transform;
            transformTrees = GameObject.Find("Trees").transform;
            tilesDirty = new List<WorldTile>();
            base.tilesList = new List<WorldTile>();
            base.citiesList = new List<City>();
            base.units = new ActorContainer();
            base.buildings = new BuildingContainer();
            base.job_manager_buildings = new JobManagerBuildings();
            zone_camera = new ZoneCamera();
            base.kingdoms = new KingdomManager(this);
            this.cultures = new CultureManagerpp(this);
            heat = new Heat();
            base.wind_direction = new Vector2(-0.1f, 0.2f);
            AssetManager.world_behaviours.createManagers();
            DOTween.SetTweensCapacity(1000, 100);
            return false;
        }
    }
}
