using System.Collections.Generic;
using UnityEngine;

namespace NNDynamic
{
    public class DynamicNetwork : MonoBehaviour
    {
        IExchangeNeuronData context = null;

        public NeuronalNetwork parentNetwork;
        public NeuronalNetwork nn;

        private readonly int count_InputNeurons = 12;
        private readonly int count_OuputNeurons = 4;

        readonly Vector3 hiddenNetworkSizeMin = new Vector3(1, 0, 0);
        readonly Vector3 hiddenNetworkSizeMax = new Vector3(1, 11, 0);
        readonly List<Vector3> hiddenNetworkSize = new List<Vector3>();

        // Start is called before the first frame update
        void Start()
        {
            for (int i = (int)hiddenNetworkSizeMin.x; i <= (int)hiddenNetworkSizeMax.x; i++)
            {
                for (int j = (int)hiddenNetworkSizeMin.y; j <= (int)hiddenNetworkSizeMax.y; j++)
                {
                    hiddenNetworkSize.Add(new Vector3(i, j, 0));
                }
            }

            context = GetComponent<Creature>();

            nn = new NeuronalNetwork();

            if (parentNetwork != null)
            {
                CopyNetworkFromParent();

                MutateNetwork();
            }
            else
            {
                //First Spawn(no child)

                //Version1();

                Version2();

            }
        }

        // Update is called once per frame
        void Update()
        {
            //Input Neuronen manipulieren
            context.UpdateInputNeurons(nn.GetInputNeurons());

            //Output Neuronen Werte übergeben
            context.ReadOutputNeurons(nn.GetOutputNeurons());
        }

        private void Version1()
        {
            CreateInputNeurons();

            CreateHiddenNeurons();

            CreateOutputNeurons();

            nn.CreateFullMesh();
        }

        private void Version2()
        {
            CreateInputNeurons();

            CreateOutputNeurons();

            SpawnMassivHiddenNeurons();

            ConnectNeurons();
        }

        private void MutateNetwork()
        {
            CreateNewHiddenNeurons();

            ConnectNeurons();

            ChangeWeights();

            EnpowerWeights();

            DeleteUnusedConnections();

            //DeleteUnusedHiddenNeurons();
        }

        private void ConnectNeurons()
        {
            //hidden plus output Neurons
            List<WorkingNeuron> allWorkingNeurons = new List<WorkingNeuron>();

            foreach (WorkingNeuron hiddenNeuron in nn.GetHiddenNeurons())
            {
                allWorkingNeurons.Add(hiddenNeuron);
            }

            foreach (WorkingNeuron outputNeuron in nn.GetOutputNeurons())
            {
                allWorkingNeurons.Add(outputNeuron);
            }

            int countMutations = 10;

            for (int k = 0; k < countMutations; k++)
            {
                foreach (WorkingNeuron workingNeuron in allWorkingNeurons)
                {
                    //Allg. Bereich definieren in dem connected werden darf(nur nach links!!!)
                    Vector3 fromArea = new Vector3(workingNeuron.position.x - 1, workingNeuron.position.y - 4, 0);
                    Vector3 toArea = new Vector3(workingNeuron.position.x - 1, workingNeuron.position.y + 4, 0);

                    //bestehende Connections zu anderen Neuronen
                    List<Vector3> allConnectionPoints = new List<Vector3>();

                    foreach (Connection connection in workingNeuron.connections)
                    {
                        allConnectionPoints.Add(connection.Neuron.position);
                    }

                    //Positionen aller input + hidden Neuronen
                    List<Vector3> allInputPlusHiddenNeuronsPoints = new List<Vector3>();

                    foreach (InputNeuron inputNeuron in nn.GetInputNeurons())
                    {
                        allInputPlusHiddenNeuronsPoints.Add(inputNeuron.position);
                    }

                    foreach (WorkingNeuron hiddenNeuron2 in nn.GetHiddenNeurons())
                    {
                        allInputPlusHiddenNeuronsPoints.Add(hiddenNeuron2.position);
                    }

                    //Bereich auf Positionen aller hidden Neurons abgleichen
                    for (int i = (int)fromArea.x; i <= (int)toArea.x; i++)
                    {
                        for (int j = (int)fromArea.y; j <= (int)toArea.y; j++)
                        {
                            Vector3 vector3 = new Vector3(i, j, 0);

                            //Wenn Vector3 mit einem bestehenden input oder hidden Neuron im Bereich matched
                            //und noch keine Connection zu diesem Neuron vorhanden ist
                            //und Punkt nicht Quellpunkt
                            if (Vector3InList(vector3, allInputPlusHiddenNeuronsPoints) && !Vector3InList(vector3, allConnectionPoints) && vector3 != workingNeuron.position)
                            {
                                workingNeuron.AddConnection(new Connection(FindNeuronByPosition(vector3), Random.Range(-1f, 1f)));
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void DeleteUnusedHiddenNeurons()
        {
            List<Vector3> hiddenNeuronsToBeDeleted = new List<Vector3>();

            foreach (WorkingNeuron hiddenNeuron in nn.GetHiddenNeurons())
            {
                if(hiddenNeuron.connections.Count == 0)
                {
                    hiddenNeuronsToBeDeleted.Add(hiddenNeuron.position);
                }
            }

            foreach (Vector3 vector3 in hiddenNeuronsToBeDeleted)
            {
                nn.DeleteHidden(vector3);
            }
        }

        private void DeleteUnusedConnections()
        {
            //hidden
            foreach (WorkingNeuron hiddenNeuron in nn.GetHiddenNeurons())
            {
                List<Neuron> connectionToBeDeleted = new List<Neuron>();

                foreach (Connection connection in hiddenNeuron.connections)
                {
                    if (connection.Weight > -0.01 && connection.Weight < 0.01)
                    {
                        connectionToBeDeleted.Add(connection.Neuron);
                    }
                }

                foreach (Neuron neuron in connectionToBeDeleted)
                {
                    hiddenNeuron.DeleteConnection(neuron);
                }
            }

            //output
            foreach (WorkingNeuron outputNeuron in nn.GetOutputNeurons())
            {
                List<Neuron> connectionToBeDeleted = new List<Neuron>();

                foreach (Connection connection in outputNeuron.connections)
                {
                    if (connection.Weight > -0.01 && connection.Weight < 0.01)
                    {
                        connectionToBeDeleted.Add(connection.Neuron);
                    }
                }

                foreach (Neuron neuron in connectionToBeDeleted)
                {
                    outputNeuron.DeleteConnection(neuron);
                }
            }
        }

        private void EnpowerWeights()
        {
            List<WorkingNeuron> allWorkingNeurons = new List<WorkingNeuron>();

            foreach (WorkingNeuron hiddenNeuron in nn.GetHiddenNeurons())
            {
                allWorkingNeurons.Add(hiddenNeuron);
            }

            foreach (WorkingNeuron outputNeuron in nn.GetOutputNeurons())
            {
                allWorkingNeurons.Add(outputNeuron);
            }

            foreach (WorkingNeuron workingNeuron in allWorkingNeurons)
            {
                foreach (Connection connection in workingNeuron.connections)
                {
                    if (connection.performanceCalculated)
                    {
                        float connectionWeight = connection.Weight;
                        float connectionModi = connection.activationWithinDurationAverageValue;
                        float developRate = 0.3f;

                        if (connectionModi <= 0)
                        {
                            connection.Weight = (connection.Weight * (1 - developRate));
                        }
                        else if (connectionModi > 0)
                        {
                            connection.Weight = (connection.Weight * (1 + developRate));
                        }
                    }
                }
            }
        }

        private void CreateNewHiddenNeurons()
        {
            int countMutations = 4;

            for (int i = 0; i < countMutations; i++)
            {
                int rndNeuron = Random.Range(0, nn.GetHiddenNeurons().Count);
                WorkingNeuron nextToNeuron = nn.GetHiddenNeurons()[rndNeuron];

                SpawnHiddenNeuron(nextToNeuron.position);
            }
        }

        private void SpawnHiddenNeuron(Vector3 position)
        {
            //Allg. Bereich definieren in dem gespawnt werden darf
            Vector3 fromArea = new Vector3(position.x - 1, position.y - 1, 0);
            Vector3 toArea = new Vector3(position.x + 1, position.y + 1, 0);

            //Allg. Spawnbereich mit Spielfeld abgleichen
            List<Vector3> possibleSpawnPoints = new List<Vector3>();

            for (int i = (int)fromArea.x; i <= (int)toArea.x; i++)
            {
                for (int j = (int)fromArea.y; j <= (int)toArea.y; j++)
                {
                    Vector3 vector = new Vector3(i, j, 0);

                    //Wenn Punkt im Spielfeld und Punkt nicht Quellpunkt
                    if (Vector3InList(vector, hiddenNetworkSize) && vector != position)
                    {
                        possibleSpawnPoints.Add(vector);
                    }
                }
            }

            //Allg. Spawnbereich mit bestehenden Neuronen abgleichen
            List<Vector3> allHiddenNeuronsPoints = new List<Vector3>();

            foreach (WorkingNeuron hiddenNeuron in nn.GetHiddenNeurons())
            {
                allHiddenNeuronsPoints.Add(hiddenNeuron.position);
            }

            foreach (Vector3 vector3 in possibleSpawnPoints)
            {
                if (!Vector3InList(vector3, allHiddenNeuronsPoints))
                {
                    nn.CreateNewHidden(vector3);
                    break;
                }
            }
        }

        private bool Vector3InList(Vector3 vector, List<Vector3> list)
        {
            foreach (Vector3 vector3 in list)
            {
                if(vector3 == vector)
                {
                    return true;
                }
            }
            return false;
        }

        //private void SpawnHiddenNeuron(Vector3 position)
        //{
        //    Vector3 newPosition;
        //    do
        //    {
        //        int rndX = Random.Range((int)position.x - maxSpaceBetweenNeurons, (int)position.x + maxSpaceBetweenNeurons);
        //        int rndY = Random.Range((int)position.y - maxSpaceBetweenNeurons, (int)position.y + maxSpaceBetweenNeurons);
        //        int rndZ = Random.Range((int)position.z - maxSpaceBetweenNeurons, (int)position.z + maxSpaceBetweenNeurons);
        //        newPosition = new Vector3(rndX, rndY, rndZ);
        //    } while (
        //        //Neue Position zu nah an der Position des Neurons
        //        (newPosition.x > (position.x - minSpaceBetweenNeurons) && newPosition.x < (position.x + minSpaceBetweenNeurons)) ||
        //        (newPosition.y > (position.y - minSpaceBetweenNeurons) && newPosition.y < (position.y + minSpaceBetweenNeurons)) ||
        //        (newPosition.z > (position.z - minSpaceBetweenNeurons) && newPosition.z < (position.z + minSpaceBetweenNeurons))
        //    );

        //    nn.CreateNewHidden(newPosition);
        //}

        public void SpawnMassivHiddenNeurons()
        {
            foreach (Vector3 vector3 in hiddenNetworkSize)
            {
                nn.CreateNewHidden(vector3);
            }
        }

        private void ChangeWeights()
        {
            List<Connection> allConnections = new List<Connection>();

            foreach (WorkingNeuron hiddenNeuron in parentNetwork.GetHiddenNeurons())
            {
                foreach (Connection connection in hiddenNeuron.connections)
                {
                    allConnections.Add(connection);
                }
            }

            foreach (WorkingNeuron outputNeuron in parentNetwork.GetOutputNeurons())
            {
                foreach (Connection connection in outputNeuron.connections)
                {
                    allConnections.Add(connection);
                };
            }

            int countMutations = 5;
            float developRate = 0.3f;

            for (int i = 0; i < countMutations; i++)
            {
                int rndConnection = Random.Range(0, allConnections.Count);
                int plusOrMinus = Random.Range(0, 1);

                if (plusOrMinus == 1)
                {
                    allConnections[rndConnection].Weight = (allConnections[rndConnection].Weight + developRate);
                }
                else
                {
                    allConnections[rndConnection].Weight = (allConnections[rndConnection].Weight - developRate);
                }
            }
        }

        private void CopyNetworkFromParent()
        {
            //Input Neuronen kopieren und zusätzlich in "allParentNeurons" ablegen
            foreach (InputNeuron inputNeuron in parentNetwork.GetInputNeurons())
            {
                nn.CreateNewInput(inputNeuron.position);
            }

            //Hidden Neuronen kopieren und zusätzlich in "allParentNeurons" ablegen
            foreach (WorkingNeuron hiddenNeuron in parentNetwork.GetHiddenNeurons())
            {
                nn.CreateNewHidden(hiddenNeuron.position);
            }

            //Output Neuronen kopieren und zusätzlich in "allParentNeurons" ablegen
            foreach (WorkingNeuron outputNeuron in parentNetwork.GetOutputNeurons())
            {
                nn.CreateNewOuptput(outputNeuron.position);
            }

            //Connections aller hidden Neurons kopieren
            for (int i = 0; i < parentNetwork.GetHiddenNeurons().Count; i++)
            {
                foreach (Connection connection in parentNetwork.GetHiddenNeurons()[i].connections)
                {
                    Neuron newNeuron = FindNeuronByPosition(connection.Neuron.position);
                    Connection newConnection = new Connection(newNeuron, connection.Weight)
                    {
                        activationWithinDurationAverageValue = connection.activationWithinDurationAverageValue
                    };
                    nn.GetHiddenNeurons()[i].connections.Add(newConnection);
                }
            }

            //Connections aller output Neurons kopieren
            for (int i = 0; i < parentNetwork.GetOutputNeurons().Count; i++)
            {
                foreach (Connection connection in parentNetwork.GetOutputNeurons()[i].connections)
                {
                    Neuron newNeuron = FindNeuronByPosition(connection.Neuron.position);
                    Connection newConnection = new Connection(newNeuron, connection.Weight)
                    {
                        activationWithinDurationAverageValue = connection.activationWithinDurationAverageValue
                    };
                    nn.GetOutputNeurons()[i].connections.Add(newConnection);
                }
            }
        }

        Neuron FindNeuronByPosition(Vector3 position)
        {
            List<Neuron> allNeurons = new List<Neuron>();

            foreach (InputNeuron inputNeuron in nn.GetInputNeurons())
            {
                allNeurons.Add(inputNeuron);
            }

            foreach (WorkingNeuron hiddenNeuron in nn.GetHiddenNeurons())
            {
                allNeurons.Add(hiddenNeuron);
            }

            foreach (WorkingNeuron outputNeuron in nn.GetOutputNeurons())
            {
                allNeurons.Add(outputNeuron);
            }

            foreach (Neuron neuron in allNeurons)
            {
                if (neuron.position == position) { return neuron; }
            }
            return null;
        }

        private void CreateInputNeurons()
        {
            for (int i = 0; i < count_InputNeurons; i++)
            {
                nn.CreateNewInput(new Vector3(0,i,0));
            }
        }

        private void CreateHiddenNeurons()
        {
            for (int i = 0; i < count_InputNeurons - 1; i++)
            {
                nn.CreateNewHidden(new Vector3(1, i,0));
            }
        }

        private void CreateOutputNeurons()
        {
            for (int i = 0; i < count_OuputNeurons; i++)
            {
                nn.CreateNewOuptput(new Vector3(hiddenNetworkSizeMax.x + 1, i + 4, 0));
            }
        }
    }

    public class NeuronalNetwork
    {
        readonly List<InputNeuron> inputNeurons = new List<InputNeuron>();
        readonly List<WorkingNeuron> hiddenNeurons = new List<WorkingNeuron>();
        readonly List<WorkingNeuron> outputNeurons = new List<WorkingNeuron>();

        public List<InputNeuron> GetInputNeurons()
        {
            return inputNeurons;
        }

        public List<WorkingNeuron> GetHiddenNeurons()
        {
            return hiddenNeurons;
        }

        public List<WorkingNeuron> GetOutputNeurons()
        {
            return outputNeurons;
        }

        public InputNeuron CreateNewInput(Vector3 position)
        {
            InputNeuron _in = new InputNeuron
            {
                position = position
            };
            inputNeurons.Add(_in);
            return _in;
        }

        public WorkingNeuron CreateNewHidden(Vector3 position)
        {
            WorkingNeuron hn = new WorkingNeuron
            {
                position = position
            };
            hiddenNeurons.Add(hn);
            return hn;
        }

        public WorkingNeuron CreateNewOuptput(Vector3 position)
        {
            WorkingNeuron wn = new WorkingNeuron
            {
                position = position
            };
            outputNeurons.Add(wn);
            return wn;
        }

        public void DeleteHidden(Vector3 position)
        {
            foreach (WorkingNeuron workingNeuron in hiddenNeurons)
            {
                if(workingNeuron.position == position)
                {
                    hiddenNeurons.Remove(workingNeuron);
                    break;
                }
            }
        }

        public void CreateFullMesh()
        {
            if (hiddenNeurons.Count == 0)
            {
                foreach (WorkingNeuron wn in outputNeurons)
                {
                    foreach (InputNeuron _in in inputNeurons)
                    {
                        wn.AddConnection(new Connection(_in, 0));
                    }
                }
            }
            else
            {
                foreach (WorkingNeuron wn in outputNeurons)
                {
                    foreach (WorkingNeuron hidden in hiddenNeurons)
                    {
                        wn.AddConnection(new Connection(hidden, Random.Range(-1f, 1f)));
                    }
                }

                foreach (WorkingNeuron hidden in hiddenNeurons)
                {
                    foreach (InputNeuron _in in inputNeurons)
                    {
                        hidden.AddConnection(new Connection(_in, Random.Range(-1f, 1f)));
                    }
                }
            }
        }

        public void ConnectInputToOuput()
        {
            foreach (WorkingNeuron wn in outputNeurons)
            {
                foreach (InputNeuron _in in inputNeurons)
                {
                    wn.AddConnection(new Connection(_in, Random.Range(-1f, 1f)));
                }
            }
        }
    }

    public class Connection
    {
        private Neuron neuron;
        private float weight;

        public bool performanceCalculated = false;
        float lastTimeScoring;
        readonly float durationUntilScoring = 3f;
        float activatedWithinDuration;
        public float activationWithinDurationAverageValue;

        public Neuron Neuron { get => neuron; set => neuron = value; }
        public float Weight { get => weight; set => weight = value; }

        public void CalculatePerformance()
        {
            activatedWithinDuration += 1;
            if ((Time.realtimeSinceStartup - lastTimeScoring) > durationUntilScoring)
            {
                activationWithinDurationAverageValue = activatedWithinDuration / durationUntilScoring;
                activatedWithinDuration = 0f;
                lastTimeScoring = Time.realtimeSinceStartup;
                if(performanceCalculated == false) { performanceCalculated = true; }
            }
        }

        public Connection(Neuron neuron, float weight)
        {
            this.neuron = neuron;
            this.weight = weight;
        }

        public float GetValue()
        {
            return neuron.GetValue() * weight;
        }
    }

    public class InputNeuron : Neuron
    {
        private float value = 0;

        public override float GetValue()
        {
            CalculatePerformance();

            if(value >= 1f) { activated = true; } else { activated = false; }

            return value;
        }

        public void SetValue(float value)
        {
            this.value = value;
        }
    }

    public abstract class Neuron
    {
        public Vector3 position;
        public bool activated;
        public abstract float GetValue();

        float lastTimeScoring;
        readonly float durationUntilScoring = 3f;
        float activatedWithinDuration;
        public float activationWithinDurationAverageValue;

        public void CalculatePerformance()
        {
            activatedWithinDuration += 1;
            if ((Time.realtimeSinceStartup - lastTimeScoring) > durationUntilScoring)
            {
                activationWithinDurationAverageValue = activatedWithinDuration / durationUntilScoring;
                activatedWithinDuration = 0f;
                lastTimeScoring = Time.realtimeSinceStartup;
            }
        }

    }

    public class WorkingNeuron : Neuron
    {
        public float value = 0;
        public float lastSum = 0;
        readonly public List<Connection> connections = new List<Connection>();
        private readonly IActivationFunction activationFunction = new Boolean();
        public override float GetValue()
        {
            float sum = 0;

            foreach (Connection connection in connections)
            {
                sum += connection.GetValue();
                if (connection.Neuron.activated)
                {
                    connection.CalculatePerformance();
                }
            }

            lastSum = sum;

            value = activationFunction.Activation(sum);

            CalculatePerformance();

            if (value >= 1f) { activated = true; } else { activated = false; }

            return value;
        }

        public void AddConnection(Connection c)
        {
            connections.Add(c);
        }

        public void DeleteConnection(Neuron neuron)
        {
            foreach (Connection connection in connections)
            {
                if(connection.Neuron == neuron)
                {
                    connections.Remove(connection);
                    break;
                }          
            }
        }
    }

    interface IActivationFunction
    {
        //public static Boolean ActivationBoolean = new Boolean();

        float Activation(float input);
        float Derivative(float input);
    }

    class Identity : IActivationFunction
    {
        public float Activation(float input)
        {
            return input;
        }

        public float Derivative(float input)
        {
            return 1;
        }
    }

    class Boolean : IActivationFunction
    {
        public float Activation(float input)
        {
            if (input < 0) { return 0; } else { return 1; }
        }

        public float Derivative(float input)
        {
            return 1;
        }
    }

    class Sigmoid : IActivationFunction
    {
        public float Activation(float input)
        {
            return 1f / (1f + (float)System.Math.Pow(System.Math.E, -input));
        }

        public float Derivative(float input)
        {
            float sigmoid = Activation(input);
            return sigmoid * (1 - sigmoid);
        }
    }

    class HyperbolicTangent : IActivationFunction
    {
        public float Activation(float input)
        {
            double epx = System.Math.Pow(System.Math.E, input);
            double enx = System.Math.Pow(System.Math.E, -input);

            return (float)(epx - enx) / (float)(epx + enx);
        }

        public float Derivative(float input)
        {
            float tanh = Activation(input);
            return 1 - tanh * tanh;
        }
    }

    public interface IExchangeNeuronData
    {
        void UpdateInputNeurons(List<InputNeuron> neurons);

        void ReadOutputNeurons(List<WorkingNeuron> neurons);
    }
}


