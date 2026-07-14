import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useAdminData } from "@/lib/admin-data";

export function AdminUserDialog({
  open,
  onOpenChange,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const { upsertAdmin } = useAdminData();

  const [userId, setUserId] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  async function handleSubmit() {
    const value = userId.trim();

    if (!value) {
      setError("Admin ID required");
      return;
    }

    try {
      setSaving(true);

      const res = await fetch("/api/admin", {
        method: "POST",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
        body: JSON.stringify({
          userId: value,
        }),
      });

      const saved = await res.json();

      if (!res.ok) {
        throw new Error(saved?.message || `HTTP ${res.status}`);
      }

      // ✅ backend should return:
      // {
      //   id,
      //   fullName,
      //   addedBy,
      //   addedOn
      // }
      upsertAdmin(saved);

      setUserId("");
      setError(null);
      onOpenChange(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to add admin");
    } finally {
      setSaving(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-sm">
        <DialogHeader>
          <DialogTitle>Add Admin</DialogTitle>
        </DialogHeader>

        <div className="space-y-2">
          <Label>Admin ID</Label>

          <Input
            value={userId}
            onChange={(e) => {
              setUserId(e.target.value);
              if (error) setError(null);
            }}
            onKeyDown={(e) => {
              if (e.key === "Enter" && !saving) {
                void handleSubmit();
              }
            }}
            placeholder="Enter Admin ID"
            autoFocus
            disabled={saving}
          />

          {error && (
            <p className="text-xs text-destructive">{error}</p>
          )}
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={saving}
          >
            Cancel
          </Button>

          <Button onClick={handleSubmit} disabled={saving}>
            {saving ? "Adding..." : "Add"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}