import os
from openai import OpenAI

# 1. Initialize the client using your GLM-5.2 provider's endpoint
# (Swap URL if using OpenRouter, TogetherAI, or native GLM provider)
client = OpenAI(
    base_url="https://integrate.api.nvidia.com/v1", 
    api_key="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"
)

def load_file(filename):
    """Safely reads local files for context."""
    if not os.path.exists(filename):
        raise FileNotFoundError(f"Could not find required file: {filename}")
    with open(filename, "r", encoding="utf-8") as f:
        return f.read()

try:
    # 2. Automatically load all your migration workspace assets into memory
    # Adjust paths if these files live in subfolders (e.g., "src/ChartController.cs")
    chart_code = load_file("ChartsController.cs")
    program_code = load_file("HomeController.cs")
    my_csv = load_file("usage-report-2026-07-15 (2).csv")
    client_csv = load_file("MyReport_UsageData_2026-07-15.csv")

    # 3. Build a comprehensive prompt detailing the migration requirements
    analysis_prompt = f"""
You are an expert backend engineer handling a legacy migration project from ASP.NET to .NET 10 and a React frontend.

GOAL:
I need my newly generated frontend CSV (usage-report.csv) to match the client's legacy backend-generated CSV (MyReport.csv) perfectly. The end-result data outputs must be completely identical.

CONTEXTUAL FILES:
Here is the legacy client backend logic that manages and manipulates these charts:
--- START CLIENT CHART CONTROLLER ---
{chart_code}
--- END CLIENT CHART CONTROLLER ---

--- START CLIENT PROGRAM CONTROLLER ---
{program_code}
--- END CLIENT PROGRAM CONTROLLER ---

DATA OUTPUTS:
--- START MY CSV (usage-report.csv) ---
{my_csv}
--- END MY CSV ---

--- START CLIENT CSV (MyReport.csv) ---
{client_csv}
--- END CLIENT CSV ---

TASKS REQUIRED:
1. Cross-examine both CSV strings. Point out exactly where they diverge (e.g., column sorting, row alignment, missing headers, datetime formats, float precision/rounding differences).
2. Trace those data discrepancies back to the provided C# logic. What specific filtering, data transformations, conditional math, or defaults inside ChartController.cs or ProgramController.cs did I fail to implement in my new frontend logic?
3. Provide clear step-by-step instructions or pseudo-code modifications I need to apply to my code so my output matches theirs perfectly.
"""

    print("🚀 Sending migration workspace files to GLM-5.2...")
    
    # 4. Request the deep contextual evaluation
    response = client.chat.completions.create(
        model="z-ai/glm-5.2",
        messages=[{"role":"user","content":analysis_prompt}],
        temperature=1,
        top_p=1,
        max_tokens=16384,
        seed=42,
        
        stream=True
    )
    print("\n📊 === GLM-5.2 MIGRATION COMPARISON ANALYSIS ===\n")
    print(response.choices[0].message.content)

except FileNotFoundError as err:
    print(f"❌ Error: {err}")
except Exception as e:
    print(f"❌ API Error: {str(e)}")