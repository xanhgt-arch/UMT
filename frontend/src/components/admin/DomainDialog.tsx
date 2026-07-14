import { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useAdminData, newDomId } from "@/lib/admin-data";
import { REGIONS } from "@/lib/mock-data";
import type { DomainRecord, Region } from "@/lib/types";

export function DomainDialog({
  open,
  onOpenChange,
  initial,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  initial: DomainRecord | null;
}) {
  const { domainRecords , upsertDomainRecord} = useAdminData();

  const [form, setForm] = useState<DomainRecord>(blank(domainRecords));

  const [selectedRegion, setSelectedRegion] = useState<Region | "">("");

  const [isNewDomain, setIsNewDomain] = useState(false);
  const [isNewRegion, setIsNewRegion] = useState(false);

  useEffect(() => {
    if (open) {
      const newForm = initial ? { ...initial } : blank(domainRecords);

      setForm(newForm);

      
      if (!initial) {
        setIsNewDomain(false);
        setIsNewRegion(false);
      }


      setSelectedRegion(newForm.region);

      // ✅ handle domain mode
      if (
        newForm.technicalDomain &&
        !domainRecords.some(
          (d) =>
            d.region === newForm.region &&
            d.technicalDomain === newForm.technicalDomain
        )
      ) {
        setIsNewDomain(true);
      } else {
        setIsNewDomain(false);
      }

      // ✅ handle region mode
      if (newForm.region && !REGIONS.includes(newForm.region)) {
        setIsNewRegion(true);
      } else {
        setIsNewRegion(false);
      }
    }
  }, [open, initial, domainRecords]);

  const isEdit = initial != null;

  // ✅ Unique domains per region
  const domainsForRegion = Array.from(
    new Set(
      domainRecords
        .filter((d) => d.region === selectedRegion)
        .map((d) => d.technicalDomain)
    )
  );

  function set<K extends keyof DomainRecord>(key: K, value: DomainRecord[K]) {
    setForm((prev) => ({ ...prev, [key]: value }));
  }

  function validate(): boolean {
    const next: Partial<Record<keyof DomainRecord, string>> = {};

    if (!form.region) next.region = "Region required";
    if (!form.technicalDomain) next.technicalDomain = "Domain required";
    if (!form.fullName) next.fullName = "UserId required";

    return Object.keys(next).length === 0;
  }

  async function handleSubmit() {
    if (!validate()) return;

    const payload = {
      userId: form.fullName,
      domain: form.technicalDomain,
      region: form.region,
    };

    const options: RequestInit = {
      method: isEdit ? "PUT" : "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(payload),
    };

    let response;

    if (isEdit) {
      response = await fetch(`/api/domain/${form.fullName}`, options);
    } else {
      response = await fetch(`/api/domain`, options);
    }

    // ✅ IMPORTANT: get backend response (audit fields)
    const saved = await response.json().catch(() => null);

    // ✅ fallback if backend doesn't return full object
    const updatedRecord = {
      ...form,
      ...(saved || {}),
    };

    upsertDomainRecord(updatedRecord);

    onOpenChange(false);
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>
            {isEdit ? "Edit domain mapping" : "Add domain mapping"}
          </DialogTitle>
          <DialogDescription>
            Mapping based on region → domain → UserId.
          </DialogDescription>
        </DialogHeader>

        <div className="grid gap-4 py-2 sm:grid-cols-2">

          {/* ✅ REGION */}
          <Field label="Region">
            <Select
              value={isNewRegion ? "__new__" : form.region}
              onValueChange={(v) => {
                if (v === "__new__") {
                  setIsNewRegion(true);
                  set("region", "" as Region);
                  setSelectedRegion("");
                } else {
                  setIsNewRegion(false);
                  set("region", v as Region);
                  setSelectedRegion(v as Region);
                  set("technicalDomain", "");
                }
              }}
            >
              <SelectTrigger className="h-10 rounded-xl">
                <SelectValue placeholder="Select region" />
              </SelectTrigger>

              <SelectContent>
                {REGIONS.map((r) => (
                  <SelectItem key={r} value={r}>
                    {r}
                  </SelectItem>
                ))}

                <SelectItem value="__new__">
                  + Add new region
                </SelectItem>
              </SelectContent>
            </Select>
          </Field>

          {/* ✅ NEW REGION INPUT */}
          {isNewRegion && (
            <Field label="New Region">
              <Input
                value={isNewRegion ? "__new__" : form.region}
                onChange={(e) => {
                  const val = e.target.value;
                  set("region", val as Region);
                  setSelectedRegion(val as Region);
                }}
                placeholder="Enter new region"
              />
            </Field>
          )}

          {/* ✅ DOMAIN */}
          <Field label="Domain">
            <Select
              value={isNewDomain ? "__new__" : form.technicalDomain}
              disabled={!selectedRegion}
              onValueChange={(v) => {
                if (v === "__new__") {
                  setIsNewDomain(true);
                  set("technicalDomain", "");
                } else {
                  setIsNewDomain(false);
                  set("technicalDomain", v);
                }
              }}
            >
              <SelectTrigger className="h-10 rounded-xl">
                <SelectValue placeholder="Select domain" />
              </SelectTrigger>

              <SelectContent>
                {domainsForRegion.map((d) => (
                  <SelectItem key={d} value={d}>
                    {d}
                  </SelectItem>
                ))}

                <SelectItem value="__new__">
                  + Add new domain
                </SelectItem>
              </SelectContent>
            </Select>
          </Field>

          {/* ✅ NEW DOMAIN INPUT */}
          {isNewDomain && selectedRegion && (
            <Field label="New Domain">
              <Input
                value={isNewDomain ? "__new__" : form.technicalDomain}
                onChange={(e) =>
                  set("technicalDomain", e.target.value)
                }
                placeholder="Enter new domain"
              />
            </Field>
          )}

          {/* ✅ USER ID */}
          <Field label="User ID">
            
            <Input
              value={form.fullName || ""}
              disabled={isEdit || !form.technicalDomain}
              onChange={(e) => set("fullName", e.target.value)}
              placeholder="Enter User ID"
            />

          </Field>

          {/* ✅ EMAIL PREVIEW */}
          {form.fullName && (
            <div className="text-xs text-muted-foreground sm:col-span-2">
              {form.fullName.toLowerCase()}@cooperstandard.com
            </div>
          )}

        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>

          <Button onClick={handleSubmit}>
            {isEdit ? "Save changes" : "Add mapping"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

function Field({
  label,
  children,
}: {
  label: string;
  children: React.ReactNode;
}) {
  return (
    <div className="space-y-1.5">
      <Label className="text-xs font-medium text-muted-foreground">
        {label}
      </Label>
      {children}
    </div>
  );
}

function blank(existing: DomainRecord[]): DomainRecord {
  return {
    id: newDomId(existing),
    fullName: "",
    technicalDomain: "",
    corporateGroup: "",
    region: "" as Region,
  } as DomainRecord;
}