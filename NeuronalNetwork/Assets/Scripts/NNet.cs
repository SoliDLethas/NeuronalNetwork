using System.Collections.Generic;
using UnityEngine;

namespace NNTest
{
    public class NNet : MonoBehaviour
    {
        public I_ExchangeNeuronData context;

        public List<float> fixedWeights = new List<float>();

        public NeuralNetwork nn;

        public int nw_wide = 3;
        public int nw_high = 4;

        // Start is called before the first frame update
        void Start()
        {
            nn = new NeuralNetwork();

            TestModel t = new TestModel();

            for (int i = 0; i < nw_high; i++)
            {
                nn.createNewInput(0, i);
                nn.createNewOuptput(nw_wide - 1, i);
            }

            nn.createHiddenNeurons((nw_wide - 2) * nw_high);

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
            context.UpdateInputNeurons(nn.inputNeurons);

            //Output Neuronen Werte übergeben
            context.ReadOutputNeurons(nn.outputNeurons);
        }

        public class NeuralNetwork
        {
            public List<InputNeuron> inputNeurons = new List<InputNeuron>();
            public List<WorkingNeuron> hiddenNeurons = new List<WorkingNeuron>();
            public List<WorkingNeuron> outputNeurons = new List<WorkingNeuron>();

            public WorkingNeuron createNewOuptput(int x, int y)
            {
                WorkingNeuron wn = new WorkingNeuron();
                wn.x = x;
                wn.y = y;
                wn.neuronType = NeuronType.WorkingNeuron;
                outputNeurons.Add(wn);
                return wn;
            }

            public void reset()
            {
                foreach (WorkingNeuron wn in hiddenNeurons)
                {
                    wn.reset();
                }

                foreach (WorkingNeuron wn in outputNeurons)
                {
                    wn.reset();
                }
            }

            public InputNeuron createNewInput(int x, int y)
            {
                InputNeuron _in = new InputNeuron();
                _in.x = x;
                _in.y = y;
                _in.neuronType = NeuronType.InputNeuron;
                inputNeurons.Add(_in);
                return _in;
            }

            public void createHiddenNeurons(int amount)
            {
                for (int i = 0; i < amount; i++)
                {
                    WorkingNeuron hn = new WorkingNeuron();
                    hn.x = 1;
                    hn.y = i;
                    hn.neuronType = NeuronType.WorkingNeuron;
                    hiddenNeurons.Add(hn);
                }
            }

            public void backpropagation(float[] shoulds, float epsilon)
            {
                if (shoulds.Length != outputNeurons.Count)
                {
                    throw new System.Exception();
                }

                reset();

                for (int i = 0; i < shoulds.Length; i++)
                {
                    outputNeurons[i].calculateOutputDelta(shoulds[i]);
                }

                if (hiddenNeurons.Count > 0)
                {
                    for (int i = 0; i < shoulds.Length; i++)
                    {
                        outputNeurons[i].backpropagateSmallDelta();
                    }
                }

                for (int i = 0; i < shoulds.Length; i++)
                {
                    outputNeurons[i].deltaLearning(epsilon);
                }

                for (int i = 0; i < hiddenNeurons.Count; i++)
                {
                    hiddenNeurons[i].deltaLearning(epsilon);
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

            public List<Neuron> getAllNeurons()
            {
                List<Neuron> allNeurons = new List<Neuron>();

                foreach (Neuron neuron in inputNeurons)
                {
                    allNeurons.Add(neuron);
                }

                foreach (Neuron neuron1 in hiddenNeurons)
                {
                    allNeurons.Add(neuron1);
                }

                foreach (Neuron neuron2 in outputNeurons)
                {
                    allNeurons.Add(neuron2);
                }

                return allNeurons;
            }

            public List<WorkingNeuron> getWorkingNeurons()
            {
                List<WorkingNeuron> allNeurons = new List<WorkingNeuron>();

                foreach (WorkingNeuron neuron1 in hiddenNeurons)
                {
                    allNeurons.Add(neuron1);
                }

                foreach (WorkingNeuron neuron2 in outputNeurons)
                {
                    allNeurons.Add(neuron2);
                }

                return allNeurons;
            }
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
        public int x;
        public int y;
        public NeuronType neuronType;
        public abstract float getValue();
    }

    public class WorkingNeuron : Neuron
    {
        public List<Connection> connections = new List<Connection>();
        private ActivationFunction activationFunction = new Boolean();
        private float smallDelta = 0;
        private float value = 0;
        private bool valueClean = false;
        public override float getValue()
        {
            //if (!valueClean)
            //{
            float sum = 0;

            foreach (Connection connection in connections)
            {
                sum += connection.getValue();
            }

            value = activationFunction.activation(sum);
            valueClean = true;
            //}

            return value;
        }

        public void reset()
        {
            valueClean = false;
            smallDelta = 0;
        }

        public void addConnection(Connection c)
        {
            connections.Add(c);
        }

        public void deltaLearning(float epsilon)
        {
            float bigDeltaFactor = activationFunction.derivative(getValue()) * epsilon * smallDelta;

            for (int i = 0; i < connections.Count; i++)
            {
                float bigDelta = bigDeltaFactor * connections[i].getNeuron().getValue();
                connections[i].addWeight(bigDelta);
            }
        }

        public void calculateOutputDelta(float should)
        {
            smallDelta = should - getValue();
        }

        public void backpropagateSmallDelta()
        {
            foreach (Connection c in connections)
            {
                Neuron n = c.getNeuron();
                if (n.GetType() == typeof(WorkingNeuron))
                {
                    WorkingNeuron wn = (WorkingNeuron)n;
                    wn.smallDelta += this.smallDelta * c.getWeight();
                }
            }
        }
    }

    public class TestModel
    {
        public TestModel()
        {
            Model m = new Model();
            m.addLayer(Model.LayerType.Input, 8);
            m.addLayer(Model.LayerType.Serial, 7);
            m.addLayer(Model.LayerType.Serial, 6);
            m.addLayer(Model.LayerType.Serial, 5);
            m.addLayer(Model.LayerType.Cross, 5);
            m.addLayer(Model.LayerType.Serial, 4);

            Model m2 = new Model();
            m2.copyFromModel(m);
        }
    }

    public class Model
    {
        private List<Layer> layers = new List<Layer>();

        public void addLayer(LayerType layerType, int countNeurons, List<float> weights = null)
        {

            if (layerType == LayerType.Input)
            {
                Input i = new Input();
                i.model = this;
                i.addNeurons(countNeurons);
                layers.Add(i);
                i.connectNeurons();
            }
            else if (layerType == LayerType.Serial)
            {
                Serial s = new Serial();
                s.model = this;
                s.addNeurons(countNeurons);
                layers.Add(s);
                s.connectNeurons(weights);
            }
            else if (layerType == LayerType.Cross)
            {
                Cross c = new Cross();
                c.model = this;
                c.addNeurons(countNeurons);
                layers.Add(c);
                c.connectNeurons(weights);
            }
        }

        public void copyFromModel(Model model)
        {
            foreach (Layer layer in model.layers)
            {
                List<InputNeuron> previousInputNeurons = new List<InputNeuron>();
                List<WorkingNeuron> previousWorkingNeurons = new List<WorkingNeuron>();

                if (layer.GetType() == typeof(Input))
                {
                    Input i = (Input)layer;
                    previousInputNeurons = i.inputNeurons;
                    addLayer(LayerType.Input, previousInputNeurons.Count);
                }
                else if (layer.GetType() == typeof(Serial))
                {
                    Serial s = (Serial)layer;
                    previousWorkingNeurons = s.workingNeurons;

                    List<float> weights = new List<float>();
                    foreach (WorkingNeuron wn in previousWorkingNeurons)
                    {
                        foreach (Connection connection in wn.connections)
                        {
                            weights.Add(connection.getWeight());
                        }
                    }

                    addLayer(LayerType.Serial, previousWorkingNeurons.Count, weights);
                }
                else if (layer.GetType() == typeof(Cross))
                {
                    Cross c = (Cross)layer;
                    previousWorkingNeurons = c.workingNeurons;

                    List<float> weights = new List<float>();
                    foreach (WorkingNeuron wn in previousWorkingNeurons)
                    {
                        foreach (Connection connection in wn.connections)
                        {
                            weights.Add(connection.getWeight());
                        }
                    }

                    addLayer(LayerType.Cross, previousWorkingNeurons.Count, weights);
                }
            }
        }

        public List<InputNeuron> getInputNeurons()
        {
            if (layers[0].GetType() == typeof(Input))
            {
                Input i = (Input)layers[0];
                return i.inputNeurons;
            }
            else
            {
                throw new System.Exception();
            }
        }

        public List<WorkingNeuron> getOutputNeurons()
        {
            int last = layers.Count;

            if (layers[last].GetType() == typeof(Serial))
            {
                Serial s = (Serial)layers[last];
                return s.workingNeurons;
            }
            else if (layers[last].GetType() == typeof(Cross))
            {
                Cross c = (Cross)layers[last];
                return c.workingNeurons;
            }
            else
            {
                throw new System.Exception();
            }
        }

        public enum LayerStage { Previous }

        public abstract class Layer
        {
            public Model model;

            public int layerStage;
            public abstract void addNeurons(int count);
            public abstract void connectNeurons(List<float> weights = null);

            public Layer getLayer(Layer context, LayerStage layerStage)
            {
                if (layerStage == LayerStage.Previous)
                {
                    int currentIndex = model.layers.IndexOf(context);
                    return model.layers[currentIndex - 1];
                }
                else
                {
                    throw new System.Exception();
                }
            }
        }

        public enum LayerType { Input, Serial, Cross }

        public class Input : Layer
        {
            public List<InputNeuron> inputNeurons = new List<InputNeuron>();

            public override void addNeurons(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    InputNeuron _in = new InputNeuron();
                    inputNeurons.Add(_in);
                }
            }

            public override void connectNeurons(List<float> weights = null)
            {
                //Do Nothing
            }
        }

        public class Serial : Layer
        {
            public List<WorkingNeuron> workingNeurons = new List<WorkingNeuron>();
            public override void addNeurons(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    WorkingNeuron wn = new WorkingNeuron();
                    workingNeurons.Add(wn);
                }
            }

            public override void connectNeurons(List<float> weights = null)
            {
                Layer previousLayer = getLayer(this, LayerStage.Previous);

                List<InputNeuron> previousInputNeurons = new List<InputNeuron>();
                List<WorkingNeuron> previousWorkingNeurons = new List<WorkingNeuron>();

                int index = 0;

                if (previousLayer.GetType() == typeof(Input))
                {
                    Input i = (Input)previousLayer;
                    previousInputNeurons = i.inputNeurons;

                    for (int j = 0; j < workingNeurons.Count; j++)
                    {
                        if (weights != null)
                        {
                            workingNeurons[j].addConnection(new Connection(previousInputNeurons[j], weights[index++]));
                            workingNeurons[j].addConnection(new Connection(previousInputNeurons[j + 1], weights[index++]));
                        }
                        else
                        {
                            workingNeurons[j].addConnection(new Connection(previousInputNeurons[j], Random.Range(-1f, 1f)));
                            workingNeurons[j].addConnection(new Connection(previousInputNeurons[j + 1], Random.Range(-1f, 1f)));
                        }
                    }
                }
                else if (previousLayer.GetType() == typeof(Serial))
                {
                    Serial s = (Serial)previousLayer;
                    previousWorkingNeurons = s.workingNeurons;

                    for (int i = 0; i < workingNeurons.Count; i++)
                    {
                        if (weights != null)
                        {
                            workingNeurons[i].addConnection(new Connection(previousWorkingNeurons[i], weights[index++]));
                            workingNeurons[i].addConnection(new Connection(previousWorkingNeurons[i + 1], weights[index++]));
                        }
                        else
                        {
                            workingNeurons[i].addConnection(new Connection(previousWorkingNeurons[i], Random.Range(-1f, 1f)));
                            workingNeurons[i].addConnection(new Connection(previousWorkingNeurons[i + 1], Random.Range(-1f, 1f)));
                        }
                    }
                }
                else if (previousLayer.GetType() == typeof(Cross))
                {
                    Cross c = (Cross)previousLayer;
                    previousWorkingNeurons = c.workingNeurons;

                    for (int i = 0; i < workingNeurons.Count; i++)
                    {
                        if (weights != null)
                        {
                            workingNeurons[i].addConnection(new Connection(previousWorkingNeurons[i], weights[index++]));
                            workingNeurons[i].addConnection(new Connection(previousWorkingNeurons[i + 1], weights[index++]));
                        }
                        else
                        {
                            workingNeurons[i].addConnection(new Connection(previousWorkingNeurons[i], Random.Range(-1f, 1f)));
                            workingNeurons[i].addConnection(new Connection(previousWorkingNeurons[i + 1], Random.Range(-1f, 1f)));
                        }
                    }
                }
            }
        }

        public class Cross : Layer
        {
            public List<WorkingNeuron> workingNeurons = new List<WorkingNeuron>();
            public override void addNeurons(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    WorkingNeuron wn = new WorkingNeuron();
                    workingNeurons.Add(wn);
                }
            }

            public override void connectNeurons(List<float> weights = null)
            {
                Layer previousLayer = getLayer(this, LayerStage.Previous);

                List<InputNeuron> previousInputNeurons = new List<InputNeuron>();
                List<WorkingNeuron> previousWorkingNeurons = new List<WorkingNeuron>();

                int index = 0;

                if (previousLayer.GetType() == typeof(Input))
                {
                    Input i = (Input)previousLayer;
                    previousInputNeurons = i.inputNeurons;

                    foreach (WorkingNeuron _wn in workingNeurons)
                    {
                        foreach (InputNeuron _in in previousInputNeurons)
                        {
                            if (weights != null)
                            {
                                _wn.addConnection(new Connection(_in, weights[index++]));
                            }
                            else
                            {
                                _wn.addConnection(new Connection(_in, Random.Range(-1f, 1f)));
                            }

                        }
                    }
                }
                else if (previousLayer.GetType() == typeof(Serial))
                {
                    Serial s = (Serial)previousLayer;
                    previousWorkingNeurons = s.workingNeurons;

                    foreach (WorkingNeuron _wn in workingNeurons)
                    {
                        foreach (WorkingNeuron wn in previousWorkingNeurons)
                        {
                            if (weights != null)
                            {
                                _wn.addConnection(new Connection(wn, weights[index++]));
                            }
                            else
                            {
                                _wn.addConnection(new Connection(wn, Random.Range(-1f, 1f)));
                            }

                        }
                    }
                }
                else if (previousLayer.GetType() == typeof(Cross))
                {
                    Cross c = (Cross)previousLayer;
                    previousWorkingNeurons = c.workingNeurons;

                    foreach (WorkingNeuron _wn in workingNeurons)
                    {
                        foreach (WorkingNeuron wn in previousWorkingNeurons)
                        {
                            if (weights != null)
                            {
                                _wn.addConnection(new Connection(wn, weights[index++]));
                            }
                            else
                            {
                                _wn.addConnection(new Connection(wn, Random.Range(-1f, 1f)));
                            }

                        }
                    }
                }
            }
        }
    }

    public interface ActivationFunction
    {
        //public static Boolean ActivationBoolean = new Boolean();

        float activation(float input);
        float derivative(float input);
    }

    public class id : ActivationFunction
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

    public class Boolean : ActivationFunction
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

    public class Sigmoid : ActivationFunction
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

    public class HyperbolicTangent : ActivationFunction
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

    public enum NeuronType { InputNeuron, WorkingNeuron }
}