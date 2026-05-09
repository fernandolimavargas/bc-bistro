import { createFileRoute } from "@tanstack/react-router";
import { useMemo, useState } from "react";
import { BistroHeader } from "@/components/BistroHeader";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import {
  addVenda,
  formatBRL,
  getProdutos,
  type Categoria,
  type ItemVenda,
  type Produto,
} from "@/lib/bistro-store";
import { Minus, Plus, ShoppingBag, Trash2, Check } from "lucide-react";
import { toast } from "sonner";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { title: "BC Bistro — Bistrô da Igreja" },
      { name: "description", content: "Sistema de vendas do BC Bistro." },
    ],
  }),
  component: Loja,
});

const CATEGORIAS: Categoria[] = ["Almoços", "Hambúrgueres", "Bebidas", "Doces"];

function Loja() {
  const [produtos, setProdutos] = useState<Produto[]>(() => getProdutos());
  const [carrinho, setCarrinho] = useState<Record<string, number>>({});
  const [filtro, setFiltro] = useState<Categoria | "Todos">("Todos");

  const visiveis = useMemo(
    () => (filtro === "Todos" ? produtos : produtos.filter((p) => p.categoria === filtro)),
    [produtos, filtro]
  );

  const itens: ItemVenda[] = useMemo(
    () =>
      Object.entries(carrinho)
        .map(([id, qtd]) => {
          const p = produtos.find((x) => x.id === id);
          if (!p || qtd <= 0) return null;
          return { produtoId: p.id, nome: p.nome, preco: p.preco, quantidade: qtd };
        })
        .filter(Boolean) as ItemVenda[],
    [carrinho, produtos]
  );

  const total = itens.reduce((s, i) => s + i.preco * i.quantidade, 0);

  const add = (id: string) => setCarrinho((c) => ({ ...c, [id]: (c[id] ?? 0) + 1 }));
  const sub = (id: string) =>
    setCarrinho((c) => {
      const n = (c[id] ?? 0) - 1;
      const next = { ...c };
      if (n <= 0) delete next[id];
      else next[id] = n;
      return next;
    });
  const remove = (id: string) =>
    setCarrinho((c) => {
      const next = { ...c };
      delete next[id];
      return next;
    });

  const finalizar = () => {
    if (itens.length === 0) return;
    addVenda(itens);
    setCarrinho({});
    toast.success("Venda finalizada!", { description: `Total: ${formatBRL(total)}` });
    setProdutos(getProdutos());
  };

  return (
    <div className="min-h-screen bg-background">
      <BistroHeader />

      <main className="mx-auto grid max-w-6xl gap-6 px-4 py-8 lg:grid-cols-[1fr_360px]">
        <section>
          <div className="mb-6">
            <h1 className="font-display text-4xl font-bold">Cardápio</h1>
            <p className="text-muted-foreground">
              Toque em um produto para adicionar ao carrinho.
            </p>
          </div>

          <div className="mb-5 flex flex-wrap gap-2">
            {(["Todos", ...CATEGORIAS] as const).map((c) => (
              <button
                key={c}
                onClick={() => setFiltro(c)}
                className={`rounded-full border px-4 py-1.5 text-sm transition ${
                  filtro === c
                    ? "border-foreground bg-foreground text-primary-foreground"
                    : "border-border bg-card hover:border-foreground/40"
                }`}
              >
                {c}
              </button>
            ))}
          </div>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-3">
            {visiveis.map((p) => {
              const qtd = carrinho[p.id] ?? 0;
              return (
                <Card
                  key={p.id}
                  className="group flex flex-col justify-between p-5 transition hover:border-[color:var(--gold)] hover:shadow-md"
                >
                  <div>
                    <div className="text-[10px] font-semibold uppercase tracking-widest text-[color:var(--gold)]">
                      {p.categoria}
                    </div>
                    <h3 className="mt-1 font-display text-lg font-semibold leading-snug">
                      {p.nome}
                    </h3>
                    <div className="mt-2 text-2xl font-bold">{formatBRL(p.preco)}</div>
                  </div>
                  <div className="mt-4 flex items-center justify-between">
                    {qtd > 0 ? (
                      <div className="flex items-center gap-2">
                        <Button size="icon" variant="outline" onClick={() => sub(p.id)}>
                          <Minus className="h-4 w-4" />
                        </Button>
                        <span className="w-6 text-center font-semibold">{qtd}</span>
                        <Button size="icon" variant="outline" onClick={() => add(p.id)}>
                          <Plus className="h-4 w-4" />
                        </Button>
                      </div>
                    ) : (
                      <span className="text-xs text-muted-foreground">Disponível</span>
                    )}
                    <Button onClick={() => add(p.id)} size="sm">
                      <Plus className="mr-1 h-4 w-4" /> Adicionar
                    </Button>
                  </div>
                </Card>
              );
            })}
            {visiveis.length === 0 && (
              <p className="text-muted-foreground">Nenhum produto nesta categoria.</p>
            )}
          </div>
        </section>

        <aside className="lg:sticky lg:top-6 lg:h-fit">
          <Card className="overflow-hidden">
            <div className="flex items-center gap-2 border-b border-border bg-foreground px-5 py-4 text-primary-foreground">
              <ShoppingBag className="h-5 w-5 text-[color:var(--gold)]" />
              <h2 className="font-display text-xl">Carrinho</h2>
              <span className="ml-auto text-sm text-[color:var(--gold-soft)]">
                {itens.length} {itens.length === 1 ? "item" : "itens"}
              </span>
            </div>

            <div className="max-h-[50vh] divide-y divide-border overflow-y-auto">
              {itens.length === 0 && (
                <p className="px-5 py-10 text-center text-sm text-muted-foreground">
                  Seu carrinho está vazio.
                </p>
              )}
              {itens.map((i) => (
                <div key={i.produtoId} className="flex items-center gap-3 px-5 py-3">
                  <div className="flex-1">
                    <div className="text-sm font-medium leading-tight">{i.nome}</div>
                    <div className="text-xs text-muted-foreground">
                      {i.quantidade} × {formatBRL(i.preco)}
                    </div>
                  </div>
                  <div className="font-semibold">{formatBRL(i.preco * i.quantidade)}</div>
                  <button
                    onClick={() => remove(i.produtoId)}
                    className="text-muted-foreground hover:text-destructive"
                    aria-label="Remover"
                  >
                    <Trash2 className="h-4 w-4" />
                  </button>
                </div>
              ))}
            </div>

            <div className="border-t border-border bg-card px-5 py-4">
              <div className="mb-3 flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Total</span>
                <span className="font-display text-2xl font-bold">{formatBRL(total)}</span>
              </div>
              <Button
                className="w-full bg-[color:var(--gold)] text-foreground hover:bg-[color:var(--gold)]/90"
                size="lg"
                disabled={itens.length === 0}
                onClick={finalizar}
              >
                <Check className="mr-2 h-4 w-4" /> Finalizar venda
              </Button>
            </div>
          </Card>
        </aside>
      </main>
    </div>
  );
}
