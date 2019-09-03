using System.Collections.Generic;
using UnityEngine;

public class SimpleNetwork : MonoBehaviour
{
    I_ExchangeNeuronData context = null;

    public List<float> fixedWeights = new List<float>();

    public NeuralNetwork nn;

    int count_InputNeurons = 12;
    int count_OuputNeurons = 4;

    // Start is called before the first frame update
    void Start()
    {

        //context = GetComponent<CreatureV2>();

        nn = new NeuralNetwork();

        for (int i = 0; i < count_InputNeurons; i++)
        {
            nn.createNewInput();
        }

        for (int j = 0; j < count_OuputNeurons; j++)
        {
            nn.createNewOuptput();
        }

        nn.createHiddenNeurons(count_InputNeurons + 1);

        if (fixedWeights.Count > 0)
        {
            nn.createFullMesh(fixedWeights.ToArray());
        }
        else
        {
            nn.createFullMesh();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Input Neuronen manipulieren
        context.UpdateInputNeurons(nn.getInputNeurons());

        //Output Neuronen Werte übergeben
        context.ReadOutputNeurons(nn.getOutputNeurons());
    }

    public class NeuralNetwork
    {
        List<InputNeuron> inputNeurons = new List<InputNeuron>();
        List<WorkingNeuron> hiddenNeurons = new List<WorkingNeuron>();
        List<WorkingNeuron> outputNeurons = new List<WorkingNeuron>();

        public List<InputNeuron> getInputNeurons()
        {
            return inputNeurons;
        }

        public List<WorkingNeuron> getHiddenNeurons()
        {
            return hiddenNeurons;
        }

        public List<WorkingNeuron> getOutputNeurons()
        {
            return outputNeurons;
        }

        public WorkingNeuron createNewOuptput()
        {
            WorkingNeuron wn = new WorkingNeuron();
            outputNeurons.Add(wn);
            return wn;
        }

        public InputNeuron createNewInput()
        {
            InputNeuron _in = new InputNeuron();
            inputNeurons.Add(_in);
            return _in;
        }

        public void createHiddenNeurons(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                WorkingNeuron hn = new WorkingNeuron();
                hiddenNeurons.Add(hn);
            }
        }

        public void createFullMesh()
        {
            if (hiddenNeurons.Count == 0)
            {
                foreach (WorkingNeuron wn in outputNeurons)
                {
                    foreach (InputNeuron _in in inputNeurons)
                    {
                        wn.addConnection(new Connection(_in, 0));
                    }
                }
            }
            else
            {
                foreach (WorkingNeuron wn in outputNeurons)
                {
                    foreach (WorkingNeuron hidden in hiddenNeurons)
                    {
                        wn.addConnection(new Connection(hidden, Random.Range(-1f, 1f)));
                    }
                }

                foreach (WorkingNeuron hidden in hiddenNeurons)
                {
                    foreach (InputNeuron _in in inputNeurons)
                    {
                        hidden.addConnection(new Connection(_in, Random.Range(-1f, 1f)));
                    }
                }
            }
        }

        public void createFullMesh(float[] weights)
        {
            if (hiddenNeurons.Count == 0)
            {
                if (weights.Length != inputNeurons.Count * outputNeurons.Count)
                {
                    throw new System.Exception();
                }

                int index = 0;

                foreach (WorkingNeuron wn in outputNeurons)
                {
                    foreach (InputNeuron _in in inputNeurons)
                    {
                        wn.addConnection(new Connection(_in, weights[index++]));
                    }
                }
            }
            else
            {
                if (weights.Length != inputNeurons.Count * hiddenNeurons.Count + hiddenNeurons.Count * outputNeurons.Count)
                {
                    throw new System.Exception();
                }

                int index = 0;

                foreach (WorkingNeuron hidden in hiddenNeurons)
                {
                    foreach (InputNeuron _in in inputNeurons)
                    {
                        hidden.addConnection(new Connection(_in, weights[index++]));
                    }
                }

                foreach (WorkingNeuron _out in outputNeurons)
                {
                    foreach (WorkingNeuron hidden in hiddenNeurons)
                    {
                        _out.addConnection(new Connection(hidden, weights[index++]));
                    }
                }
            }
        }

        public List<float> getAllValues()
        {
            List<float> tmp_weights = new List<float>();

            foreach (WorkingNeuron neuron in hiddenNeurons)
            {
                foreach (Connection connection in neuron.connections)
                {
                    tmp_weights.Add(connection.getWeight());
                }
            }

            foreach (WorkingNeuron neuron in outputNeurons)
            {
                foreach (Connection connection in neuron.connections)
                {
                    tmp_weights.Add(connection.getWeight());
                }
            }

            return tmp_weights;
        }
    }

    public class Connection
    {
        private Neuron neuron;
        private float weight;

        public Connection(Neuron neuron, float weight)
        {
            this.neuron = neuron;
            this.weight = weight;
        }

        public float getValue()
        {
            return neuron.getValue() * weight;
        }

        public void addWeight(float weightDelta)
        {
            weight += weightDelta;
        }

        public float getWeight()
        {
            return weight;
        }

        public Neuron getNeuron()
        {
            return neuron;
        }
    }

    public class InputNeuron : Neuron
    {
        private float value = 0;

        public override float getValue()
        {
            return value;
        }

        public void setValue(float value)
        {
            this.value = value;
        }
    }

    public abstract class Neuron
    {
        public abstract float getValue();
    }

    public class WorkingNeuron : Neuron
    {
        public List<Connection> connections = new List<Connection>();
        private ActivationFunction activationFunction = new Boolean();
        private float value = 0;
        public override float getValue()
        {
            float sum = 0;

            foreach (Connection connection in connections)
            {
                sum += connection.getValue();
            }

            value = activationFunction.activation(sum);


            return value;
        }

        public void addConnection(Connection c)
        {
            connections.Add(c);
        }
    }

    interface ActivationFunction
    {
        //public static Boolean ActivationBoolean = new Boolean();

        float activation(float input);
        float derivative(float input);
    }

    class Identity : ActivationFunction
    {
        public float activation(float input)
        {
            return input;
        }

        public float derivative(float input)
        {
            return 1;
        }
    }

    class Boolean : ActivationFunction
    {
        public float activation(float input)
        {
            if (input < 0) { return 0; } else { return 1; }
        }

        public float derivative(float input)
        {
            return 1;
        }
    }

    class Sigmoid : ActivationFunction
    {
        public float activation(float input)
        {
            return 1f / (1f + (float)System.Math.Pow(System.Math.E, -input));
        }

        public float derivative(float input)
        {
            float sigmoid = activation(input);
            return sigmoid * (1 - sigmoid);
        }
    }

    class HyperbolicTangent : ActivationFunction
    {
        public float activation(float input)
        {
            double epx = System.Math.Pow(System.Math.E, input);
            double enx = System.Math.Pow(System.Math.E, -input);

            return (float)(epx - enx) / (float)(epx + enx);
        }

        public float derivative(float input)
        {
            float tanh = activation(input);
            return 1 - tanh * tanh;
        }
    }

    public interface I_ExchangeNeuronData
    {
        void UpdateInputNeurons(List<InputNeuron> neurons);

        void ReadOutputNeurons(List<WorkingNeuron> neurons);
    }
}
