public class TypeChart
{
   static float[,] chart = new float[18, 18]
    {   /*              NOR   FIR   WAT   ELE   GRA   ICE   FIG   POI   GRO   FLY   PSY   BUG   ROC   GHO   DRA   DAR   STE   FAI */            
        /* Normal */    {1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   0f,   1f,   1f,   1f,   1f},
        /* Fire */      {1f, 0.5f, 0.5f,   1f,   2f,   2f,   1f,   1f,   1f,   1f,   1f,   2f, 0.5f,   1f, 0.5f,   1f,   2f,   1f},
        /* Water */     {1f,   2f, 0.5f,   1f, 0.5f,   1f,   1f,   1f,   2f,   1f,   1f,   1f,   2f,   1f, 0.5f,   1f,   1f,   1f},
        /* Electric */  {1f,   1f,   2f, 0.5f, 0.5f,   1f,   1f,   1f,   0f,   2f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   1f},
        /* Grass */     {1f, 0.5f,   2f,   1f, 0.5f,   1f,   1f, 0.5f,   2f, 0.5f,   1f, 0.5f,   2f,   1f, 0.5f,   1f, 0.5f,   1f},
        /* Ice */       {1f, 0.5f, 0.5f,   1f,   2f, 0.5f,   1f,   1f,   2f,   2f,   1f,   1f,   1f,   1f,   2f,   1f, 0.5f,   1f},
        /* Fighting */  {2f,   1f,   1f,   1f,   1f,   2f,   1f, 0.5f,   1f, 0.5f, 0.5f, 0.5f,   2f,   0f,   1f,   2f,   2f, 0.5f},
        /* Poison */    {1f,   1f,   1f,   1f,   2f,   1f,   1f, 0.5f, 0.5f,   1f,   1f,   1f, 0.5f, 0.5f,   1f,   1f,   0f,   2f},
        /* Ground */    {1f,   2f,   1f,   2f, 0.5f,   1f,   1f,   2f,   1f,   0f,   1f, 0.5f,   2f,   1f,   1f,   1f,   2f,   1f},
        /* Flying */    {1f,   1f,   1f, 0.5f,   2f,   1f,   2f,   1f,   1f,   1f,   1f,   2f, 0.5f,   1f,   1f,   1f, 0.5f,   1f},
        /* Psychic */   {1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   1f,   1f, 0.5f,   1f,   1f,   1f,   1f,   0f, 0.5f,   1f},
        /* Bug */       {1f, 0.5f,   1f,   1f,   2f,   1f, 0.5f, 0.5f,   1f, 0.5f,   2f,   1f,   1f, 0.5f,   1f,   2f, 0.5f, 0.5f},
        /* Rock */      {1f,   2f,   1f,   1f,   1f,   2f, 0.5f,   1f, 0.5f,   2f,   1f,   2f,   1f,   1f,   1f,   1f, 0.5f,   1f},
        /* Ghost */     {0f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f, 0.5f,   1f,   1f},
        /* Dragon */    {1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f, 0.5f,   0f},
        /* Dark */      {1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f, 0.5f,   1f, 0.5f},
        /* Steel */     {1f, 0.5f, 0.5f, 0.5f,   1f,   2f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f, 0.5f,   2f},
        /* Fairy */     {1f, 0.5f,   1f,   1f,   1f,   1f,   2f, 0.5f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f, 0.5f,   1f}
    };

    public static float GetEffectiveness(MonsterType attacking, MonsterType defending)
    {
        if (attacking == MonsterType.None || defending == MonsterType.None)
        {
            return 1;
        }

        return chart[(int)attacking - 1, (int)defending - 1];
    }
}
