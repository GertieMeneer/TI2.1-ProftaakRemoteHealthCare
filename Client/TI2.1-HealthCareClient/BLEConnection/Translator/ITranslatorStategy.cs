namespace TI2._1_HealthCareClient.BLEConnection.Translator {
    public interface ITranslatorStrategy {
       FEData Translate(byte[] data);
    }
}