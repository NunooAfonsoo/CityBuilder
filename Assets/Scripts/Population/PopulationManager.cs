using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Population
{
    public class PopulationManager : MonoBehaviour
    {
        public static PopulationManager Instance { get; private set; }

        public HashSet<Person> IdlePeople { get; private set; }
        public HashSet<Person> TreeChoppers { get; private set; }
        public HashSet<Person> StoneMiners { get; private set; }

        // Only for inspector reasons
        private List<Person> IdlePersonList;
        private List<Person> TreeChoppersList;
        private List<Person> StoneMinersList;

        //////DICTIONARY WITH PRIORITIES FOR EACH JOB? KEY JOB, VALUE PRIORITY?//////

        private void Awake()
        {
            Instance = this;

            IdlePeople = new HashSet<Person>();
            TreeChoppers = new HashSet<Person>();
            StoneMiners = new HashSet<Person>();

            // Only for inspector reasons
            IdlePersonList = new List<Person>();
            TreeChoppersList = new List<Person>();
            StoneMinersList = new List<Person>();
        }

        public void PersonSpawned(Person person)
        {
            person.OnStateChanged += Person_OnStateChanged;
            AddPerson(person, person.CurrentState);
        }

        private void Person_OnStateChanged(object sender, Person.StateChangedArgs e)
        {
            RemovePerson(sender as Person, e.oldState);
            AddPerson(sender as Person, e.newState);
        }

        private void RemovePerson(Person person, Person.PersonStates state)
        {
            switch (state)
            {
                case Person.PersonStates.Idling:
                    IdlePeople.Remove(person);
                    IdlePersonList = new List<Person>(IdlePeople);
                    break;
                case Person.PersonStates.ChoppingTree:
                    TreeChoppers.Remove(person);
                    TreeChoppersList = new List<Person>(TreeChoppers);
                    break;
                case Person.PersonStates.MiningStone:
                    StoneMiners.Remove(person);
                    StoneMinersList = new List<Person>(StoneMiners);
                    break;
                default:
                    break;
            }
        }

        private void AddPerson(Person person, Person.PersonStates state)
        {
            switch (state)
            {
                case Person.PersonStates.Idling:
                    IdlePeople.Add(person);
                    IdlePersonList = new List<Person>(IdlePeople);
                    break;
                case Person.PersonStates.ChoppingTree:
                    TreeChoppers.Add(person);
                    TreeChoppersList = new List<Person>(TreeChoppers);
                    break;
                case Person.PersonStates.MiningStone:
                    StoneMiners.Add(person);
                    StoneMinersList  = new List<Person>(StoneMiners);
                    break;
                default:
                    break;
            }
        }
    }
}
