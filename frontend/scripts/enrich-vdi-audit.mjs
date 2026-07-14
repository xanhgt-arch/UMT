import fs from "node:fs";
import path from "node:path";

const root = process.cwd();
const sessionsPath = path.join(root, "src", "lib", "data", "raw-sessions.json");
const vdiSourcePath = path.join(root, "src", "lib", "data", "vdi.json");
const vdiPublicPath = path.join(root, "public", "static", "vdi.json");

const SENTINEL_DATE = "2000-12-31T18:30:00.000Z";

function isMeaningfulDate(value) {
  return typeof value === "string" && value.length > 0 && value !== SENTINEL_DATE;
}

function loadJson(filePath) {
  const raw = fs.readFileSync(filePath, "utf8");
  return JSON.parse(raw);
}

const sessions = loadJson(sessionsPath);
const vdiUsers = loadJson(vdiSourcePath);

const audit = new Map();

for (const row of sessions) {
  const key = (row.UserID || "").toLowerCase();
  if (!key) continue;

  const createdDate = isMeaningfulDate(row.CreatedDate) ? row.CreatedDate : null;
  const modifiedDate = isMeaningfulDate(row.ModifiedDate) ? row.ModifiedDate : null;

  const existing = audit.get(key) ?? {
    createdDate: null,
    createdBy: null,
    modifiedDate: null,
    modifiedBy: null,
  };

  if (createdDate && (!existing.createdDate || createdDate < existing.createdDate)) {
    existing.createdDate = createdDate;
    existing.createdBy = row.CreatedBy || null;
  } else if (!existing.createdBy && row.CreatedBy) {
    existing.createdBy = row.CreatedBy;
  }

  if (modifiedDate && (!existing.modifiedDate || modifiedDate > existing.modifiedDate)) {
    existing.modifiedDate = modifiedDate;
    existing.modifiedBy = row.ModifiedBy || null;
  }

  audit.set(key, existing);
}

let enrichedCount = 0;
const enriched = vdiUsers.map((user) => {
  const trail = audit.get(user.fullName.toLowerCase());
  if (!trail) return user;
  enrichedCount += 1;
  return {
    ...user,
    createdDate: trail.createdDate,
    createdBy: trail.createdBy,
    modifiedDate: trail.modifiedDate,
    modifiedBy: trail.modifiedBy,
  };
});

const json = JSON.stringify(enriched);
fs.writeFileSync(vdiSourcePath, json);
fs.writeFileSync(vdiPublicPath, json);

console.log(
  `Enriched ${enrichedCount}/${vdiUsers.length} VDI users. Wrote: ${path.relative(
    root,
    vdiSourcePath
  )} and ${path.relative(root, vdiPublicPath)}.`
);
