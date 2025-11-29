using Microsoft.ML;
using System.IO;
using TradingSystem.ML.Models;

namespace TradingSystem.ML.Services;

public class MLPredictor : IMLPredictor
{
    private readonly MLContext _ml;
    private readonly ITransformer _model;
    private readonly PredictionEngine<PredictionInput, PredictionOutput> _engine;

    public MLPredictor(string modelPath)
    {
        _ml = new MLContext();

        if (!File.Exists(modelPath))
            throw new FileNotFoundException("ML Model not found", modelPath);

        _model = _ml.Model.Load(modelPath, out _);
        _engine = _ml.Model.CreatePredictionEngine<PredictionInput, PredictionOutput>(_model);
    }

    public PredictionOutput Predict(PredictionInput input)
    {
        return _engine.Predict(input);
    }
}